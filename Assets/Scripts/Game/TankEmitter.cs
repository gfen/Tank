using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public abstract class TankEmitter : NetworkBehaviour
    {
        public int FrameRate;

        public int BulletPower = 1;

        public event Action BulletHit;

        private int _frameDelta = 0;
        
        void FixedUpdate()
        {
            if (_frameDelta > 0) _frameDelta -= Frame.Rate;

            if (_frameDelta <= 0f)
            {
                FireUpdate();
            }
        }

        protected abstract void FireUpdate();

        protected void StartFire()
        {
            _frameDelta = FrameRate;
        }

        protected void ServerFire()
        {
            if (!isServer) return;

            Vector3 position = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 1.5f));
            Vector3 direction = transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
            BulletManager.Instance.CreateBullet(position, direction, BulletPower, tag, BulletHit);           
        }

        public void AddBulletPower()
        {
            if (BulletPower < 4)
            {
                BulletPower++;
            }
        }
    }

}
