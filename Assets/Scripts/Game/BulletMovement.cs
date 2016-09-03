using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletMovement : NetworkBehaviour
    {
        public float Speed;

        [HideInInspector]
        [SyncVar]
        public Vector3 Direction;

        private Rigidbody _rigidbody;

        public override void OnStartClient()
        {
            if (isServer) return;

            ApplyVelocity();
        }

        public void ApplyVelocity()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.velocity = Direction*Speed;
        }
    }
}
