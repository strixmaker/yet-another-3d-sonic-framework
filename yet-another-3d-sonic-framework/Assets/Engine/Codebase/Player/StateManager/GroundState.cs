using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;

public class GroundState : PlayerState
{
    bool JumpReleased;
    void Update()
    {
        if (HInput.GetButtonDown("Jump") && Player.Grounded) SetState<JumpState>();
        if (!Player.Grounded){
            if (!HInput.GetButton("Jump")) JumpReleased = true;
            if (GetState<JumpState>().AllowDoubleJump && !GetState<JumpState>().DoubleJumped && JumpReleased && HInput.GetButtonDown("Jump")){
                GetState<JumpState>().ReleasedJump = true;
                GetState<JumpState>().DoubleJump();
                SetState<JumpState>();
            }
        }
        else{
            JumpReleased = false;
        }
    }
}
