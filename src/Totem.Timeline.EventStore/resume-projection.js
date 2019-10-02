// resume-projection.js
//
// Observes the area stream and flow checkpoint streams to:
//
// 1. Link area events to a stream for each flow instance
// 2. Maintain a list of instances with pending work, to use when resuming
// 3. Maintain a schedule of pending events
//
// More information on EventStore projections:
// https://eventstore.org/docs/projections/user-defined-projections/

options({
  resultStreamName: "resume"
});

fromAll()
  .when({ $init: defaultState, $any: onNext })
  .transformBy(getResumeState)
  .outputState();

function defaultState() {
  return { checkpoint: null, schedule: {}, instances: {} };
}

function onNext(state, event) {
  let { streamId, metadata } = event;

  observe();

  function observe() {
    if(streamId.startsWith("$")) {
      return;
    }
    else if(streamId === "timeline") {
      updateArea();
    }
    else {
      if(streamId.endsWith("-checkpoint")) {
        updateProgress();
      }
    }
  }

  function updateArea() {
    state.checkpoint = parseInt(event.sequenceNumber);

    updateRoutes();
    updateSchedule();
  }

  //
  // Routes
  //

  function updateRoutes() {
    let types = metadata.routeTypes.slice(0);

    for(let { type, ids } of metadata.routeIds) {
      for(let id of ids) {
        updateMultiInstanceRoute(types[type], id);
      }

      types[type] = null;
    }

    for(let type of types) {
      if(type) {
        updateSingleInstanceRoute(type);
      }
    }
  }

  function updateSingleInstanceRoute(type) {
    linkTo(type + "-routes", event);

    updateInstanceLatest(state.instances, type);
  }

  function updateMultiInstanceRoute(type, id) {
    linkTo(`${type}|${id}-routes`, event);

    updateInstanceLatest(getInstanceIds(type), id);
  }

  function updateInstanceLatest(instances, key) {
    updateInstance(instances, key, instance => instance[0] = state.checkpoint);
  }

  function updateInstance(instances, key, update) {
    let instance = instances[key];

    if(!instance) {
      let latest = null;
      let checkpoint = null;
      let isStopped = false;

      instance = [latest, checkpoint, isStopped];

      instances[key] = instance;
    }

    update(instance);
  }

  function getInstanceIds(type) {
    let ids = state.instances[type];

    if(!ids) {
      ids = {};

      state.instances[type] = ids;
    }

    return ids;
  }

  //
  // Schedule
  //

  function updateSchedule() {
    if(metadata.whenOccurs) {
      appendToSchedule();
    }
    else {
      if(metadata.cause !== null && metadata.fromSchedule) {
        removeFromSchedule();
      }
    }
  }

  function appendToSchedule() {
    linkTo("schedule", event);

    state.schedule[state.checkpoint] = null;
  }

  function removeFromSchedule() {
    let cause = parseInt(metadata.cause);

    delete state.schedule[cause];
  }

  //
  // Progress
  //

  function updateProgress() {
    let flowKey = streamId.substring(0, streamId.length - "-checkpoint".length);

    let separatorIndex = flowKey.indexOf("|");

    if(separatorIndex === -1) {
      updateSingleInstanceProgress(flowKey);
    }
    else {
      let type = flowKey.substring(0, separatorIndex);
      let id = flowKey.substring(separatorIndex + 1);

      updateMultiInstanceProgress(type, id);
    }
  }

  function updateSingleInstanceProgress(type) {
    if(metadata.isDone) {
      delete state.instances[type];
    }
    else {
      updateInstanceProgress(state.instances, type);
    }
  }

  function updateMultiInstanceProgress(type, id) {
    let instanceIds = getInstanceIds(type);

    if(metadata.isDone) {
      delete instanceIds[id];

      if(Object.keys(instanceIds).length === 0) {
        delete state.instances[type];
      }
    }
    else {
      updateInstanceProgress(instanceIds, id);
    }
  }

  function updateInstanceProgress(instances, key) {
    updateInstance(instances, key, instance => {
      instance[1] = metadata.position;
      instance[2] = instance[2] || metadata.errorPosition !== null;
    });
  }
}

//
// Resume state
//

function getResumeState(state) {
  let routes = [];
  let schedule;

  buildRoutes();
  buildSchedule();

  return { checkpoint: state.checkpoint, routes, schedule };

  function buildRoutes() {
    for(let type in state.instances) {
      buildTypeRoutes(type, state.instances[type]);
    }
  }

  function buildSchedule() {
    schedule = Object.keys(state.schedule).map(position => parseInt(position));

    schedule.sort((a, b) => a - b);
  }

  function buildTypeRoutes(type, typeState) {
    if(Array.isArray(typeState)) {
      if(isResumable(typeState)) {
        routes.push(type);
      }
    }
    else {
      let resumableIds = Object.keys(typeState).filter(id => isResumable(typeState[id]));

      if(resumableIds.length > 0) {
        routes.push([type, ...resumableIds]);
      }
    }
  }

  function isResumable([latest, checkpoint, isStopped]) {
    return !isStopped &&
      latest !== null &&
      (checkpoint === null || checkpoint < latest);
  }
}