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
        Anim.SetBool("Bouncing", isInBounce);
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

            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 1.75f, Player.GroundRayMask))
            {
                if (hit.collider != null)
                {
                    isInBounce = false;
                    Player.Grounded = false;
                    Player.rb.velocity = Vector3.ProjectOnPlane(Player.rb.velocity, Vector3.up) * hit.normal.y + hit.normal * bounceForce[bounceIndex];
                    bounceIndex = Mathf.Min(bounceIndex + 1, bounceForce.Length - 1);
                }
            }
        }
        if (Player.Grounded) bounceIndex = 0;
    }
}
