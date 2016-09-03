using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Game
{
    public class TankLife : NetworkBehaviour
    {
        public int Life;

        public Material InvincibilityMaterial;

        public event Action<Vector3> Destroyed;

        private Renderer _helmetRenderer;

        private Material _normalMaterial;

        private bool _invincible;

        private Coroutine _changeToNormalCoroutine;

        void Start()
        {
            _invincible = false;
            _helmetRenderer = transform.Find("Model/Top").GetComponent<Renderer>();
            _normalMaterial = _helmetRenderer.sharedMaterial;
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (!isServer) return;

            if (!other.CompareTag("Bullet")) return;

            var bulletPower = other.GetComponent<BulletPower>();
            if (CompareTag(bulletPower.EmitterTag)) return;

            Destroy(other.gameObject);

            if (_invincible) return;

            ApplyDamage(bulletPower.Power);
            if (Life <= 0)
            {
                bulletPower.Hit();
                Destroy(gameObject);
                if (Destroyed != null) Destroyed(transform.position);
            }
        }

        protected virtual void ApplyDamage(int power)
        {
            Life -= power;
        }

        public void ChangeToInvincibility(float duration)
        {
            if (!isServer) return;

            ChangeToInvincibilityInternal(duration);
            RpcChangeToInvincibility(duration);
        }

        [ClientRpc]
        private void RpcChangeToInvincibility(float duration)
        {
            if (isServer) return;

            ChangeToInvincibilityInternal(duration);
        }

        private void ChangeToInvincibilityInternal(float duration)
        {
            if (_invincible) StopCoroutine(_changeToNormalCoroutine);

            _helmetRenderer.sharedMaterial = InvincibilityMaterial;
            _invincible = true;
            _changeToNormalCoroutine = StartCoroutine(ChangeToNormal(duration));
        }

        private IEnumerator ChangeToNormal(float duration)
        {
            yield return new WaitForSeconds(duration - 3);

            for (int i = 0; i < 6; i++)
            {
                if (i % 2 == 0) _helmetRenderer.sharedMaterial = _normalMaterial;
                else _helmetRenderer.sharedMaterial = InvincibilityMaterial;

                yield return new WaitForSeconds(0.5f);
            }
            _helmetRenderer.sharedMaterial = _normalMaterial;
            _invincible = false;
        }
    }
}
