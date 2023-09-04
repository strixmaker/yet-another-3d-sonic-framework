using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;

public class PlayerCommonResources : MonoBehaviour
{
    [HideInInspector]public float Speed, LateralSpeed, VerticalSpeed;

    Rigidbody rb;
    PlayerController player;
    void Start(){
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();
    }
    void FixedUpdate(){
        Speed = rb.velocity.magnitude;
        LateralSpeed = Vector3.ProjectOnPlane(rb.velocity, player.GroundNormal).magnitude;
        VerticalSpeed = (rb.velocity - Vector3.ProjectOnPlane(rb.velocity, player.GroundNormal)).magnitude;
    }
}
