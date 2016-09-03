using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class DisappearingObject : MonoBehaviour
    {
        public float DisappearTime = 10f;

        public event Action Disappeared;

        void Start()
        {
            StartCoroutine(DestroySelf());
        }

        private IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(DisappearTime);

            Destroy(gameObject);
            if (Disappeared != null) Disappeared();
        }
    }
}
