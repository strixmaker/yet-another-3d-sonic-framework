using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Global;
namespace Framework.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public Transform Target;

        public Vector3 Rotation;
        public Vector2 YRotationLimits;
        public float Sensitivity;

        public Transform RotationAnchor, PositionAnchor;

        public float CameraZDistance;
        public float LateralDistance;
        public LayerMask CollisionMask;

        Vector3 verticalOffset;

        [Range(0,1)] public float lol;

        Vector3 LerpedTargetPos, StaticTargetPos;

        void Start()
        {
            Level.Camera = this;
            Cursor.lockState = CursorLockMode.Locked;
        }
        void Update()
        {
            Rotation += (Vector3)HInput.FGetAxis2D("Look") * Sensitivity * Time.deltaTime;
            Rotation.y = Mathf.Clamp(Rotation.y, YRotationLimits.x, YRotationLimits.y);

            RotationAnchor.localRotation = Quaternion.Euler(Rotation.y, Rotation.x, 0);
            StaticTargetPos = Target.position;
            LerpedTargetPos = Vector3.Lerp(LerpedTargetPos, StaticTargetPos, 4 * Time.deltaTime);
            Vector3 targetPosition = Vector3.Lerp(StaticTargetPos, LerpedTargetPos, lol);

            verticalOffset = Vector3.Lerp(verticalOffset, !Level.Player.Grounded ? Vector3.up * Level.Player.rb.velocity.y / -50 : Vector3.zero, 8 * Time.deltaTime);
            targetPosition += verticalOffset; 
            PositionAnchor.position = targetPosition;

            lol = Mathf.Lerp(lol, 0,5 * Time.deltaTime);
        }
        void FixedUpdate(){
            CameraCollision();
        }

        void CameraCollision(){
            float z_dist = CameraZDistance;
            Vector3 CollisionOrigin = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

            if(Physics.Raycast(RotationAnchor.TransformPoint(CollisionOrigin), -RotationAnchor.forward, out RaycastHit hit, -CameraZDistance, CollisionMask)){
                if (hit.collider != null){
                    Debug.DrawLine(CollisionOrigin, hit.point);
                    z_dist = -hit.distance;
                }
            }

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z_dist + 0.4f);
        }
    }
}