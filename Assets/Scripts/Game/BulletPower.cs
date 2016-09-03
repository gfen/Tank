using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class BulletPower : NetworkBehaviour
    {
        public int Power;

        [HideInInspector]
        public string EmitterTag;

        public event Action BulletHit;
        
        void OnTriggerEnter(Collider other)
        {
            if (!isServer) return;

            if (other.CompareTag("Border")) Destroy(gameObject);

            if (other.CompareTag("Bullet"))
            {
                var bulletPower = other.GetComponent<BulletPower>();
                if (EmitterTag != bulletPower.EmitterTag)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Hit()
        {
            if (BulletHit != null) BulletHit();
        }
    }
}
