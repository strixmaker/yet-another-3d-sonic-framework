using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Player;

public class Bounce : PlayerState
{
    public bool isInBounce;

    public float downwardBounceForce;
    public float[] bounceForce;

    public int bounceIndex;
    void Start()
    {
        
    }
    void Update()
    {
        if (HInput.GetButtonDown("Bounce") && !Player.Grounded)
        {
            isInBounce = true;
        }
    }

    private void FixedUpdate()
    {
        if (isInBounce)
        {
            Player.rb.velocity = Vector3.ProjectOnPlane(Player.rb.velocity, Vector3.up) -
                                 Vector3.up * downwardBounceForce;
            
            
        }
    }
}
