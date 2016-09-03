using System;
using Framework;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class BulletManager : MonoSingleton<BulletManager>
    {
        public GameObject Bullet;

        void Awake()
        {
            _instance = this;
        }

        public void CreateBullet(Vector3 position, Vector3 direction, int power, string sourceTag, Action bulletHit)
        {
            var bullet = (GameObject)Instantiate(Bullet, position, Quaternion.LookRotation(direction));
            bullet.transform.parent = transform;

            var bulletPower = bullet.GetComponent<BulletPower>();
            bulletPower.Power = power;
            bulletPower.EmitterTag = sourceTag;
            bulletPower.BulletHit += bulletHit;

            var bulletMovement = bullet.GetComponent<BulletMovement>();
            bulletMovement.Direction = direction;
            bulletMovement.ApplyVelocity();

            NetworkServer.Spawn(bullet);
        }

        public void Clear()
        {
            SpawnHelper.ClearAll(transform);
        }
    }
}
