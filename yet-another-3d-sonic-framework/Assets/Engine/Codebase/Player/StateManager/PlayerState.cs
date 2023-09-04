using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Framework.Player{
    /// <summary>
    /// A class usable by the Player State Manager. Can be used to define situational behavior, such as for normal running, jumping, bouncing etc
    /// </summary>
    public class PlayerState : MonoBehaviour{
        [HideInInspector] public PlayerStateManager StateManager;
        [HideInInspector] public PlayerController Player;
        [HideInInspector] public PlayerCamera Camera;
        [HideInInspector] public Animator Anim;
        [HideInInspector] public PlayerCommonResources Values;

        /// <summary>
        /// If a state is passive, its code will always execute regardless of if it's the current state or not
        /// </summary>
        public bool isPassiveState;
        
        public T GetState<T>() where T : PlayerState => StateManager.GetState<T>();
        public void SetState<T>(bool ExecuteInitialEvents = true) where T : PlayerState => StateManager.SetState<T>(ExecuteInitialEvents);

        public virtual void OnEndState(){}
        public virtual void OnStartState(){}
    }
}