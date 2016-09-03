using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

namespace Game
{
    public class TankPlayerMovement : TankMovement
    {
        private Func<float, float, Vector3> _rotateMethod;

        void Awake()
        {
            _rotateMethod = RotateInThird;
        }

        protected sealed override void MovementUpdate()
        {
            if (!hasAuthority) return;

            if (Waiting) return;

            Vector3 direction;
            if (Rotate(out direction))
            {
                Waiting = true;

                CmdMove(direction);
            }
        }

        [Command]
        private void CmdMove(Vector3 direction)
        {
            ServerMove(direction);
        }

        private bool Rotate(out Vector3 direction)
        {
            float x = CrossPlatformInputManager.GetAxis("Horizontal");
            float z = CrossPlatformInputManager.GetAxis("Vertical");

            direction = _rotateMethod(x, z);

            return Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
        }

        public void SwitchFirstPerson()
        {
            _rotateMethod = RotateInFirst;
        }

        private Vector3 RotateInThird(float x, float z)
        {
            if (x > 0.1f && Mathf.Abs(z) <= x) return Vector3.right;
            else if (x < -0.1f && Mathf.Abs(z) <= -x) return Vector3.left;
            else if (z > 0.1f && Mathf.Abs(x) <= z) return Vector3.forward;
            else if (z < -0.1f && Mathf.Abs(x) <= -z) return Vector3.back;
            return Vector3.zero;
        }

        private Vector3 RotateInFirst(float x, float z)
        {
            if (x > 0.1f && Mathf.Abs(z) <= x) return transform.localToWorldMatrix.MultiplyVector(Vector3.right);
            else if (x < -0.1f && Mathf.Abs(z) <= -x) return transform.localToWorldMatrix.MultiplyVector(Vector3.left);
            else if (z > 0.1f && Mathf.Abs(x) <= z) return transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
            else if (z < -0.1f && Mathf.Abs(x) <= -z) return transform.localToWorldMatrix.MultiplyVector(Vector3.back);
            return Vector3.zero;
        }
    }
}
