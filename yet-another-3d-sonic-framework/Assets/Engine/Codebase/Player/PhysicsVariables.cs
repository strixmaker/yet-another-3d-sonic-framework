using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Framework.Player
{
    /// <summary>
    /// An object that contains various physics information that the player can use
    /// </summary>
    [CreateAssetMenu(fileName = "Physics Variables", menuName = "Sonic/Physics Variables")]
    public class PhysicsVariables : ScriptableObject
    {
        [Header("Ground Parameters")]
        public float Acceleration;
        public float Deceleration, SkiddingForce, TurnSpeed, TopSpeed, MaxSpeed;

        public float SpeedLossOnTurn;
        public AnimationCurve TurnSpeedLossMultiplierOverSpeed, TurnRateOverSpeed;

        public float GravityForceUpward, GravityForceDownward;

        [Header("Sliding")] 
        public float SlideTurnSpeed;
        public float SlideDeceleration;
        public float SlideSpeedFromIdle;
        public float GravityForceUpwardSlide, GravityForceDownwardSlide;

        [Header("Air Parameters")]
        public float GravityStrength;
        public float AirTurnSpeed, AirSkiddingForce, AirAcceleration, AirDeceleration;
        public bool ApplyDecelerationInAir;

        [Space]
        public float StartJumpStrength;
        public float JumpStrength, MaxJumpHoldTime;

        [Space]
        public bool AllowAirDrag;
        public float AirDragMinimumSpeed, AirDragRetractStrength;

    }
}
