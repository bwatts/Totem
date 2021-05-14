using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Events
{
    public abstract class EventSourced : IEventSourced
    {
        readonly Dictionary<Type, Action<IEvent>> _givensByEventType = new();
        readonly ConcurrentQueue<ErrorInfo> _errors = new();

        public IEnumerable<Type> GivenTypes => _givensByEventType.Keys;
        public long? Version { get; private set; }
        public bool HasErrors => !_errors.IsEmpty;
        public IEnumerable<ErrorInfo> Errors => _errors;

        public void Load(IEvent given, long? version = null)
        {
            if(given == null)
                throw new ArgumentNullException(nameof(given));

            if(version != null)
            {
                if(version < 0)
                    throw new ArgumentOutOfRangeException(nameof(version), $"Version must be positive");

                if(Version != null && version != Version + 1)
                    throw new ArgumentOutOfRangeException(nameof(version), $"Expected version {Version + 1} but received {version}");

                Version = version;
            }

            if(_givensByEventType.TryGetValue(given.GetType(), out var action))
            {
                action(given);
            }
        }

        protected void Given<TEvent>(Action<TEvent> action) where TEvent : IEvent
        {
            if(action == null)
                throw new ArgumentNullException(nameof(action));

            _givensByEventType[typeof(TEvent)] = e => action((TEvent) e);
        }

        protected void ThenError(ErrorInfo error)
        {
            if(error == null)
                throw new ArgumentNullException(nameof(error));

            _errors.Enqueue(error);
        }

        protected void ThenErrors(IEnumerable<ErrorInfo> errors)
        {
            if(errors == null)
                throw new ArgumentNullException(nameof(errors));

            foreach(var error in errors)
            {
                _errors.Enqueue(error);
            }
        }

        protected void ThenErrors(params ErrorInfo[] errors) =>
            ThenErrors(errors.AsEnumerable());
    }
}