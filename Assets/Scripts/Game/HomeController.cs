using System;
using UnityEngine;

namespace Game
{
    public class HomeController : MonoBehaviour
    {
        public event Action Destroyed;
        
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Bullet")) return;

            Destroy(other.gameObject);
            Destroy(gameObject);
            if (Destroyed != null) Destroyed();
        }
    }
}
