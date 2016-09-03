using UnityEngine;

namespace Game
{
    public class TankEnemyMovement : TankMovement
    {
        private static readonly Vector3[] Directions = new [] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        private bool _suspended = false;
        protected sealed override void MovementUpdate()
        {
            if (!isServer) return;

            Vector3 direction;
            if (Rotate(out direction))
            {
                ServerMove(direction);
            }
        }

        private bool Rotate(out Vector3 direction)
        {
            direction = Vector3.zero;

            if (_suspended) return false;
            
            if (Random.Range(0, 4) == 0)
            {
                direction = RotateRandomly();
                return true;
            }

            direction = transform.forward;

            return true;
        }

        private Vector3 RotateRandomly()
        {
            int random = Random.Range(0, 4);
            return Directions[random];
        }

        public void Suspend()
        {
            _suspended = true;
        }

        public void Resume()
        {
            _suspended = false;
        }
    }
}
