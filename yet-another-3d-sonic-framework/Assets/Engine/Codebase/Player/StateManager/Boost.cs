using System.Collections;
using System.Collections.Generic;
using Framework.Player;
using UnityEngine;
using Framework.Global;
public class Boost : PlayerState
{
    public float BoostSpeed;

    public bool Boosting, WasBoosting;

    public float MaxYSpeedInAir;
    void Update()
    {
        Boosting = HInput.GetButton("Boost");
        if (Boosting){
            if (!WasBoosting){
                Level.Camera.lol = .3f;
                WasBoosting = true;
            }

            Player.rb.velocity.Separate(Player.GroundNormal, out Vector3 lat, out Vector3 vert);
            Vector3 lat_dir = lat.normalized;
            lat = lat_dir * BoostSpeed;

            Player.rb.velocity = lat + vert;

            if (!Player.Grounded && Player.rb.velocity.y > MaxYSpeedInAir) Player.rb.velocity = new Vector3(Player.rb.velocity.x, Mathf.Lerp(Player.rb.velocity.y, Mathf.Min(Player.rb.velocity.y, MaxYSpeedInAir), 6 * Time.deltaTime), Player.rb.velocity.z);
        }
        else{
            if (WasBoosting){
                WasBoosting = false;
            }
        }
        Anim.SetBool("Boosting", Boosting);
    }
}
