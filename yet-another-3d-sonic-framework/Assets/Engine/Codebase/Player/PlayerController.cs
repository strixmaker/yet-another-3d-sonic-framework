using Framework.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Player
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody rb;
        public PhysicsVariables vars;

        public bool Grounded;
        [HideInInspector] public Vector3 GroundNormal, Input;
        [HideInInspector] public bool Skidding, AirSkidding;
        public LayerMask GroundRayMask;

        public float someDubiousFloat2;
        Vector3 VelocityDirection, PreviousVelocityDirection;


        public const float skidThreshold = -.55f;
        
        Vector3 vel;

        public bool AllowSliding;

        public bool Sliding;
        
        private void Awake()
        {
            Level.Player = this; //Make this the active player

            rb = GetComponent<Rigidbody>(); //initialize RB before everything else, so we avoid any NullReferenceExceptions
            GroundNormal = Vector3.up;
            VelocityDirection = PreviousVelocityDirection = Vector3.forward;
        }
        
        void Update()
        {
            Sliding = AllowSliding && HInput.GetButton("Crouch");
            
            ProcessInput();
        }

        //Order of events here
        protected void FixedUpdate()
        {
            if (Grounded)
            {
                GroundMovement();
                SlopePhysics();
            }
            else
            {
                AirMovement();
            }
            transform.up = GroundNormal;
            Collision();
        }

        //We calculate the input direction and any other overrides over here
        public void ProcessInput()
        {
            Input = VectorUtility.ComputeInput(HInput.GetAxis2D("Move"), Grounded ? GroundNormal : Vector3.up, Camera.main.transform);
        }

        /// <summary>
        /// Ground Actions
        /// </summary>
        protected void GroundMovement()
        {
            rb.velocity.Separate(GroundNormal, out Vector3 VLateral, out Vector3 VVertical); //Separate lateral and vertical velocities, on ground we will only need to manipulate the lateral vel
            if (Input != Vector3.zero && VLateral.normalized == Vector3.zero) VLateral = Input.normalized;

            //check for skidding
            if (!Skidding && Vector3.Dot(rb.velocity.normalized, Input) < skidThreshold && rb.velocity.magnitude > 5 && Input != Vector3.zero && !Sliding) Skidding = true;
            if (Vector3.Dot(rb.velocity.normalized, Input) > skidThreshold || Input == Vector3.zero || Sliding) Skidding = false;
    
            float speed = VLateral.magnitude; //Process speed changes here, from acceleration, turning etc

            if (speed == 0 && Sliding && Input != Vector3.zero)
                rb.velocity = Input.normalized * vars.SlideSpeedFromIdle;
            if (!Skidding){
                if (Input != Vector3.zero) {
                    VLateral = VLateral.magnitude * 
                                    Vector3.Lerp(VLateral.normalized, Input.normalized, ((!Sliding ? vars.TurnSpeed : vars.SlideTurnSpeed) * vars.TurnRateOverSpeed.Evaluate(rb.velocity.magnitude)) * Time.fixedDeltaTime).normalized; //This deals with turning the velocity towards the input
                }
                if (VLateral != Vector3.zero) VelocityDirection = VLateral.normalized;

                if (Input != Vector3.zero && !Sliding)
                {
                    if (speed < vars.TopSpeed) speed = Mathf.Min(speed + vars.Acceleration * Time.fixedDeltaTime, vars.TopSpeed);
                }

                if (Input == Vector3.zero || Sliding)
                {
                    speed = Mathf.Max(speed - (!Sliding ? vars.Deceleration : vars.SlideDeceleration) * Time.fixedDeltaTime, 0);
                }
            }
            else speed = Mathf.Max(speed - vars.SkiddingForce * Time.fixedDeltaTime, 0);
            
            //Handle losing speed when turning
            speed = Mathf.Max(speed -Vector3.Angle(Vector3.ProjectOnPlane(vector: VelocityDirection, GroundNormal), Vector3.ProjectOnPlane(PreviousVelocityDirection, GroundNormal)) * (vars.SpeedLossOnTurn * vars.TurnSpeedLossMultiplierOverSpeed.Evaluate(rb.velocity.magnitude)), 0);
            // ^^^ we use the delta angle between the current velocity and the previous one to make sure it still works whenever there's no input being applied
            // we project the speeds onto the ground normal just so we don't have any weird speed losses when going uphill or downhill

            VLateral = VelocityDirection * speed;
            rb.velocity = VLateral;

            PreviousVelocityDirection = VelocityDirection;
        }

        protected void AirMovement()
        {
            GroundNormal = Vector3.Lerp(GroundNormal, Vector3.up, 6 * Time.fixedDeltaTime).normalized;
            rb.velocity.Separate(Vector3.up, out Vector3 VLateral, out Vector3 VVertical);

            if (!AirSkidding && Vector3.Dot(VLateral.normalized, Input) < skidThreshold && VLateral.magnitude > 5 && Input != Vector3.zero) AirSkidding = true;
            if (Vector3.Dot(VLateral.normalized, Input) > skidThreshold || Input == Vector3.zero) AirSkidding = false;

            if (Input != Vector3.zero && !AirSkidding) VLateral = VLateral.magnitude * Vector3.RotateTowards(VLateral.normalized, Input.normalized, vars.AirTurnSpeed * Time.fixedDeltaTime, 0);
            Vector3 AVelocityDirection = VLateral.magnitude != 0 ? VLateral.normalized : Input.normalized;
            float speed = VLateral.magnitude;
            if (AirSkidding) speed -= vars.AirSkiddingForce * Time.fixedDeltaTime;
            else{
                if (Input != Vector3.zero) speed += vars.AirAcceleration * Time.fixedDeltaTime;
            }

            if (vars.AllowAirDrag) speed = AirDrag(speed);
            VVertical += Vector3.down * vars.GravityStrength * Time.fixedDeltaTime;
            VLateral = AVelocityDirection * speed;    
            rb.velocity = VLateral + VVertical;
        }

        protected float AirDrag(float speed){
            float initialSpeed = speed, finalSpeed = initialSpeed;
            if (initialSpeed > vars.AirDragMinimumSpeed) finalSpeed = Mathf.Lerp(finalSpeed, vars.AirDragMinimumSpeed, vars.AirDragRetractStrength * Time.fixedDeltaTime);
            return finalSpeed;
        }

        protected void SlopePhysics()
        {
            float slopePhysicAmp = 1 - Mathf.Abs(GroundNormal.y);
            rb.velocity += Vector3.down * slopePhysicAmp * ((Mathf.Sign(rb.velocity.y) > 0
                ? (!Sliding ? vars.GravityForceUpward : vars.GravityForceUpwardSlide)
                : (!Sliding ? vars.GravityForceDownward : vars.GravityForceDownwardSlide)) * Time.fixedDeltaTime);
        }

        protected void Collision()
        {
            if (Physics.Raycast(transform.position, Grounded ? -GroundNormal : -Vector3.up, out RaycastHit hit, Grounded ? someDubiousFloat2 : 1f, GroundRayMask))
            {
                if (hit.collider != null)
                {
                    if (Grounded)
                    {
                        float landSpeed = rb.velocity.magnitude;
                        GroundNormal = hit.normal;
                        rb.velocity.Separate(GroundNormal, out Vector3 lat, out Vector3 vert);
                        
                        rb.velocity = Vector3.ProjectOnPlane(lat + vert, GroundNormal);
                        Vector3 dir = rb.velocity.normalized;

                        rb.velocity = Vector3.MoveTowards(rb.velocity, dir * landSpeed, landSpeed * 100 * Time.deltaTime);
                        rb.position = hit.point + hit.normal * 1.1f;
                    }
                    else{
                        if (rb.velocity.y <= 0) {
                            Grounded = true;
                            GroundNormal = hit.normal;
                            rb.velocity =  Vector3.ProjectOnPlane(rb.velocity, GroundNormal);
                        }
                    }
                }
                else Grounded = false;
            }
            else Grounded = false;
        }
        
        /// <summary>
        /// The cool gizmo thingy
        /// </summary>
        public void OnDrawGizmos(){
            Gizmos.DrawLine(transform.position, transform.position - transform.up * someDubiousFloat2);
            float circleRadius = .5f;
            Vector3 origin = transform.position - transform.up, projected_velocity = Vector3.ProjectOnPlane(rb.velocity.normalized, transform.up);

#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            Handles.color = new Color(0.02f, 0.02f, 0.02f, 1f);
            Handles.DrawSolidDisc(origin, GroundNormal, circleRadius);
            Handles.DrawWireDisc(origin, GroundNormal, circleRadius);
            Handles.color = Color.Lerp(Color.red, Color.green, (Vector3.Dot(projected_velocity, Input) + 1) /2) * new Color(1,1,1,0.1f);
            Handles.DrawSolidArc(origin, GroundNormal,projected_velocity, Vector3.SignedAngle(projected_velocity, Input.normalized, GroundNormal), circleRadius - 0.05f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + projected_velocity * circleRadius);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(origin, origin + Input * circleRadius);
#endif
        }
    }

}