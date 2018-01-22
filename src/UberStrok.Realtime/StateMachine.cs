﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UberStrok.Realtime
{
    public class StateMachine<T> where T : struct, IConvertible
    {
        private State _current;

        private readonly Stack<T> _states;
        private readonly Dictionary<T, State> _registeredStates;

        public StateMachine()
        {
            _states = new Stack<T>();
            _registeredStates = new Dictionary<T, State>();
        }

        public T Current => _states.Count > 0 ? _states.Peek() : default(T);

        public void Register(T stateId, State state)
        {
            if (_registeredStates.ContainsKey(stateId))
                throw new Exception("Already contains a state handler for the specified state ID.");

            _registeredStates.Add(stateId, state);
        }

        public void Set(T stateId)
        {
            var state = default(State);
            if (!_registeredStates.TryGetValue(stateId, out state))
                throw new Exception("No state handler registered for the specified state ID.");

            if (!Current.Equals(stateId))
            {
                _current?.OnExit();

                _current = state;
                _states.Push(stateId);

                _current?.OnEnter();
            }
        }

        public void Previous()
        {
            _current?.OnExit();

            var stateId = _states.Pop();
            var state = default(State);
            var exists = _registeredStates.TryGetValue(stateId, out state);

            Debug.Assert(exists);

            _current = state;
            _current?.OnResume();
        }

        public void Reset()
        {
            while (_states.Count > 0)
                Previous();
        }

        public void Update()
        {
            _current?.OnUpdate();
        }
    }
}
