using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;

public class JumpState : PlayerState
{
    Vector3 JumpNormal;

    float JumpHoldTimer;

    public bool AllowDoubleJump;
    public float DoubleJumpStrength;

    [HideInInspector]public bool DoubleJumped, ReleasedJump;
    public override void OnStartState(){
        JumpNormal = Player.GroundNormal; //cache the surface normal to add the jump onto
        Player.Grounded = false;

        if (ReleasedJump) return;
        Player.rb.velocity += JumpNormal * Player.vars.StartJumpStrength;
        JumpHoldTimer = Player.vars.MaxJumpHoldTime;
    }

    void FixedUpdate()
    {
        if (JumpHoldTimer > 0) {
            Player.rb.velocity += JumpNormal * Player.vars.JumpStrength * Time.deltaTime;
            JumpHoldTimer -= Time.deltaTime;
        }
        else{
            if (HInput.GetButton("Jump") && !DoubleJumped && AllowDoubleJump && ReleasedJump) DoubleJump();
        }
        if (!HInput.GetButton("Jump")){
            JumpHoldTimer = 0;
            ReleasedJump = true;
        } 

        if (Player.Grounded) {
            DoubleJumped = false;
            ReleasedJump = false;
            SetState<GroundState>();
        }
    }
    public void DoubleJump(){
        JumpHoldTimer = 0;
        Anim.Play("Spin");
        GetState<GeneralState>().Spinball.GetComponent<Animator>().Play("SpinballBounce",0,0);
        Player.rb.velocity = new Vector3(Player.rb.velocity.x, DoubleJumpStrength, Player.rb.velocity.z);
        DoubleJumped = true;
    }
}
