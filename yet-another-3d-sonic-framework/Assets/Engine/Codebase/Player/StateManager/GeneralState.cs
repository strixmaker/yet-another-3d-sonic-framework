using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;

public class GeneralState : PlayerState
{
    float Tilt;
    public Transform PlayerSkin;

    public GameObject Spinball;
    public Transform SkinSpine;
    
    void Awake(){
        isPassiveState = true;
    }
    void Update()
    {
        Vector3 ProjectedVelocity = Vector3.ProjectOnPlane(Player.rb.velocity, Player.GroundNormal);
        Tilt = Mathf.Lerp(Tilt, Vector3.Dot(Anim.transform.parent.right, ProjectedVelocity.normalized) * 2, 20 * Mathf.Max(0.2f, Mathf.Abs(Vector3.Dot(Anim.transform.parent.right, ProjectedVelocity.normalized)) / 2) * Time.deltaTime);
        if (ProjectedVelocity.magnitude != 0) {
            if (Player.Grounded) Anim.transform.parent.rotation = Quaternion.Lerp(Anim.transform.parent.rotation, Quaternion.LookRotation(ProjectedVelocity, Player.GroundNormal), 13 * Time.deltaTime);
            else{
                Anim.transform.parent.rotation = !GetState<Boost>().Boosting ? Quaternion.Lerp(Anim.transform.parent.rotation, Quaternion.LookRotation(ProjectedVelocity, Player.GroundNormal), 13 * Time.deltaTime) : Quaternion.Lerp(Anim.transform.parent.rotation, Quaternion.LookRotation(Player.rb.velocity.normalized), 13 * Time.deltaTime);
            }
        }

        Anim.SetFloat("GroundedSpeed", ProjectedVelocity.magnitude);
        //Anim.SetFloat("Tilt", Tilt); //Uncomment this if you want Sonic's tilting to be based on animations instead of math
        Anim.SetBool("Grounded", Player.Grounded);
        Anim.SetBool("Slide", Player.Sliding);
        Anim.SetBool("Skidding", Player.Skidding);
        Anim.SetFloat("YSpeed", Player.rb.velocity.y);
        Anim.SetInteger("StateID", StateManager.StateID);

        Spinball.SetActive(StateManager.StateID == 1 && !GetState<Bounce>().isInBounce && !GetState<Boost>().Boosting && Anim.GetCurrentAnimatorStateInfo(0).IsName("Spin") && Player.rb.velocity.y > -5f);
    }

    void LateUpdate(){
        PlayerSkin.transform.position = transform.position;
        SkinSpine.transform.RotateAround(SkinSpine.transform.position, Anim.transform.parent.forward, Tilt * -42.5f * (Values.LateralSpeed / 70f));
    }
}
