using UnityEngine;

namespace Game
{
    public class TankEnemyEmitter : TankEmitter
    {
        public int AdditionFrameRate;

        private bool _suspended = false;

        protected override void FireUpdate()
        {
            if (_suspended) return;

            if (!isServer) return;

            int random = Random.Range(0, AdditionFrameRate);
            if (random == 0)
            {
                StartFire();
                ServerFire();
            }
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
