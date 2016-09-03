using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class ObstacleLife : NetworkBehaviour
    {
        public bool CanDamage;

        public bool CanPass;

        void OnTriggerEnter(Collider other)
        {
            if (!isServer) return;

            if (!other.CompareTag("Bullet")) return;

            if (!CanPass) Destroy(other.gameObject);
            if (CanDamage) Destroy(gameObject);
            if (!CanDamage && !CanPass)
            {
                var bulletPower = other.GetComponent<BulletPower>();
                if (bulletPower.Power > 3)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
