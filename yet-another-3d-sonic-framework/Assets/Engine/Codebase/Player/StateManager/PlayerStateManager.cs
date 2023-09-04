using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Player{
    public class PlayerStateManager : MonoBehaviour
    {
        public List<PlayerState> States = new List<PlayerState>();

        PlayerState CurrentState, PreviousState;

        [Header("References")]
        public Animator Anim;

        ///Hidden 
        [HideInInspector]public int StateID;             

        void Start(){
            foreach(PlayerState s in GetComponents<PlayerState>()){
                s.StateManager = this;
                s.Player = GetComponent<PlayerController>();
                s.Anim = Anim;
                s.Values = GetComponent<PlayerCommonResources>();
                if (!s.isPassiveState){
                    s.enabled = false;
                    States.Add(s);
                }
            }
            CurrentState = States[0];
            States[0].enabled = true;
        }

        public T GetState<T>() where T : PlayerState {
            return GetComponent<T>();
        }

        public void SetState<T>(bool ExecuteInitialEvents = true) where T : PlayerState{
            if (CurrentState != null) { CurrentState.OnEndState(); PreviousState = CurrentState; PreviousState.enabled = false; }
            if (States.Find(s=> s.GetType() == typeof(T))){ //State exists, we can transition to it
                CurrentState = States.Find(s=> s.GetType() == typeof(T));
                CurrentState.enabled = true;
                CurrentState.OnStartState();
                StateID = States.IndexOf(CurrentState);
            }
        }
    }
}