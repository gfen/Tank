using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public abstract class TankMovement : NetworkBehaviour
    {
        public float Speed;

        private Transform _modelTransform;

        private Rigidbody _rigidbody;

        private Vector3 _destination;

        private bool _waiting = false;
        public bool Waiting { get { return _waiting; } set { _waiting = value; } }

        public override void OnStartClient()
        {
            if (isServer) return;

            Init();
        }

        public override void OnStartServer()
        {
            Init();
        }

        private void Init()
        {
            _modelTransform = transform.Find("Model");

            _rigidbody = _modelTransform.GetComponent<Rigidbody>();

            _destination = _modelTransform.position;
        }
        
        void FixedUpdate()
        {
            var target = Vector3.MoveTowards(_modelTransform.position, _destination, Speed);
            _rigidbody.MovePosition(target);

            if ((_modelTransform.position - _destination).magnitude < 0.01f)
            {
                MovementUpdate();
            }
        }

        protected abstract void MovementUpdate();
        
        protected void ServerMove(Vector3 direction)
        {
            if (!isServer) return;

            bool move = MoveWithCheck(direction, CanMove);
            RpcMove(direction, move);
        }

        [ClientRpc]
        private void RpcMove(Vector3 direction, bool move)
        {
            _waiting = false;

            if (isServer) return;

            MoveWithCheck(direction, () => move);
        }

        private bool MoveWithCheck(Vector3 direction, Func<bool> check)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            if (check())
            {
                SetDestination();
                return true;
            }
            return false;
        }

        private void SetDestination()
        {
            _destination += transform.localToWorldMatrix.MultiplyVector(Vector3.forward).normalized;

            var oldPosition = transform.position;
            var oldRotation = transform.rotation;
            transform.Translate(Vector3.forward);
            _modelTransform.position = oldPosition;
            _modelTransform.rotation = oldRotation;
        }

        private bool CanMove()
        {
            Vector3[] localOrigins = { new Vector3(-0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, 0.5f) };

            Vector3[] worldOrigins = new Vector3[localOrigins.Length];
            for (int i = 0; i < localOrigins.Length; i++)
            {
                worldOrigins[i] = transform.localToWorldMatrix.MultiplyPoint3x4(localOrigins[i]);
            }

            Vector3 worldDirection = transform.localToWorldMatrix.MultiplyVector(Vector3.forward);

            Ray[] rays = new Ray[worldOrigins.Length];
            for (int i = 0; i < worldOrigins.Length; i++)
            {
                rays[i] = new Ray(worldOrigins[i], worldDirection);
            }

            for (int i = 0; i < rays.Length; i++)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(rays[i], out hitInfo, 1f))
                {
                    if (!(hitInfo.transform.CompareTag("Bullet") || hitInfo.transform.CompareTag("Tool")))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
