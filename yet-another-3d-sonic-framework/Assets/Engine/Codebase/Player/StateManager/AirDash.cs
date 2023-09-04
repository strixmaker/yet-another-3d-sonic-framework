using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;

public class AirDash : PlayerState
{
    public float DirectionalDashStrength, DirectionalTiltSpeed, AirDashDuration; 
    [Range(0.001f, 100)] public float SpeedDecayOnEnd;

    public Vector3 AirDashDirection;

    private float duration;

    private bool airDashing;

    public TrailRenderer Trail;
    void Start()
    {
        
    }
    void Update()
    {
        if (!Player.Grounded && GetState<JumpState>().ReleasedJump && HInput.GetButtonDown("AirDash"))
        {
            duration = AirDashDuration;
            AirDashDirection = Player.Input != Vector3.zero ? Player.Input : Vector3.ProjectOnPlane(Player.rb.velocity, Vector3.up).normalized;
            airDashing = true;
        }

        if (duration > 0)
        {
            if (Player.Input != Vector3.zero)
                AirDashDirection = Vector3.RotateTowards(AirDashDirection, Player.Input.normalized,
                    DirectionalTiltSpeed * Time.deltaTime,  0);
            duration = Mathf.Max(duration - Time.deltaTime, 0);
        }
        else
        {
            if (airDashing)
            {
                Player.rb.velocity /= SpeedDecayOnEnd;
                airDashing = false;
            }
        }
        Trail.emitting = airDashing;

    }

    void FixedUpdate()
    {
        if (duration > 0)
        {
            Player.rb.velocity = AirDashDirection * DirectionalDashStrength;
        }
    }
}
