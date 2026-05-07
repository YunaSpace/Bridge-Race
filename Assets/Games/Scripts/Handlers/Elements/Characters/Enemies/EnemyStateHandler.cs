using System;
using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class EnemyStateHandler : MonoBehaviour
    {
        [SerializeField] private Enemy manager;
 
        private Dictionary<Type, IEnemyState> states = new();
        private IEnemyState currentState;
#if UNITY_EDITOR
        public string PreviousState;
        public string CurrentState;
#endif

        private void Awake()
        {
            states[typeof(CollectState)] = new CollectState();
            states[typeof(ShoveState)] = new ShoveState();
            states[typeof(BridgeState)] = new BridgeState();
            states[typeof(AvoidState)] = new AvoidState();
            states[typeof(StumbleState)] = new StumbleState();
            states[typeof(GoalState)] = new GoalState();
        }

        private void Update()
        {
            if (manager.IsPaused == false)
            {
                currentState?.OnExecute(manager);
            }
        }

        public void ChangeState<T>() where T : IEnemyState
        {
            var type = typeof(T);
            var state = states[type];

            if (currentState != state)
            {
#if UNITY_EDITOR
                PreviousState = currentState?.GetType().Name;
#endif

                currentState?.OnExit(manager);
                currentState = state;
                currentState.OnEnter(manager);
#if UNITY_EDITOR
                CurrentState = currentState?.GetType().Name;
#endif
            }
        }
    }
}