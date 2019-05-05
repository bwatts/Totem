// resume-projection.js
//
// Observes the area stream and flow checkpoint streams to:
//
// 1. Link area events to a stream for each routed flow
// 2. Maintain a list of flows with pending work, to use when resuming
//
// More information on EventStore projections:
// https://eventstore.org/docs/projections/user-defined-projections/

options({
  resultStreamName: "resume"
});

fromAll()
  .when({
    $init: () => ({
      checkpoint: null,
      schedule: {},
      flows: {}
    }),
    $any: (s, e) => {
      if(e.streamId === "timeline") {
        updateArea(s, e);
      }
      else {
        tryUpdateProgress(s, e);
      }
    }
  })
  .transformBy(getResumeState)
  .outputState();

function updateArea(s, e) {
  s.checkpoint = parseInt(e.sequenceNumber);

  updateRoutes(s, e);
  updateSchedule(s, e);
}

//
// Routes
//

function updateRoutes(s, e) {
  let flows = e.metadata.routeTypes.slice(0);

  for(let { type, ids } of e.metadata.routeIds) {
    for(let id of ids) {
      updateMultiInstanceRoute(s, e, flows[type], id);
    }

    flows[type] = null;
  }

  for(let flow of flows) {
    if(flow) {
      updateSingleInstanceRoute(s, e, flow);
    }
  }
}

function updateMultiInstanceRoute(s, e, type, id) {
  linkTo(`${type}|${id}-routes`, e);

  let ids = s.flows[type];

  if(!ids) {
    ids = {};

    s.flows[type] = ids;
  }

  updateRecord(ids, id, record => {
    record[0] = s.checkpoint;
  });
}

function updateSingleInstanceRoute(s, e, type) {
  linkTo(type + "-routes", e);

  updateRecord(s.flows, type, record => {
    record[0] = s.checkpoint;
  });
}

function updateRecord(records, key, update) {
  let record = records[key];

  if(!record) {
    record = [null, null, false];

    records[key] = record;
  }

  update(record);
}

//
// Schedule
//

function updateSchedule(s, e, checkpoint) {
  if(e.metadata.whenOccurs) {
    linkTo("schedule", e);

    s.schedule[s.checkpoint] = null;
  }
  else {
    if(e.metadata.cause !== null) {
      let cause = parseInt(e.metadata.cause);

      delete s.schedule[cause];
    }
  }
}

//
// Progress
//

function tryUpdateProgress(s, e) {
  const suffix = "-checkpoint";

  if(e.streamId.endsWith(suffix)) {
    let flow = e.streamId.substring(0, e.streamId.length - suffix.length);

    let separatorIndex = flow.indexOf("|");

    if(separatorIndex === -1) {
      updateProgress(s.flows, flow, e);
    }
    else {
      let type = flow.substring(0, separatorIndex);
      let id = flow.substring(separatorIndex + 1);

      let ids = s.flows[type];

      if(!ids) {
        ids = {};

        s.flows[type] = ids;
      }

      updateProgress(ids, id, e);
    }
  }
}

function updateProgress(records, key, e) {
  updateRecord(records, key, record => {
    record[1] = e.metadata.position;
    record[2] = record[2] || e.metadata.errorPosition !== null;
  });
}

//
// Resume state
//

function getResumeState(s) {
  return {
    checkpoint: s.checkpoint,
    flows: getFlowsState(s),
    schedule: getScheduleState(s)
  };
}

function getFlowsState(s) {
  let flows = [];

  for(let type in s.flows) {
    let typeState = s.flows[type];

    if(Array.isArray(typeState)) {
      if(isResumable(typeState)) {
        flows.push(type);
      }
    }
    else {
      let resumableIds = Object.keys(typeState).filter(id => isResumable(typeState[id]));

      if(resumableIds.length > 0) {
        flows.push([type, ...resumableIds]);
      }
    }
  }

  return flows;
}

function getScheduleState(s) {
  let schedule = Object.keys(s.schedule).map(position => parseInt(position));

  schedule.sort((a, b) => a - b);

  return schedule;
}

function isResumable([latestRoute, checkpoint, stopped]) {
  return !stopped &&
    latestRoute !== null &&
    (checkpoint === null || checkpoint < latestRoute);
}