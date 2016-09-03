using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class TankPlayerToolCollector : NetworkBehaviour
    {
        public event Action AntitankGrenadeCollected;

        public event Action ClockCollected;

        public event Action HelmetCollected;

        public event Action LifeCollected;

        public event Action PowerCollected;

        public event Action SpadeCollected;
        
        void OnTriggerEnter(Collider other)
        {
            if (!isServer) return;

            if (!other.CompareTag("Tool")) return;

            var tool = other.GetComponent<Tool>();
            switch (tool.Type)
            {
                case ToolType.AntitankGrenade:
                    if (AntitankGrenadeCollected != null) AntitankGrenadeCollected();
                    break;
                case ToolType.Clock:
                    if (ClockCollected != null) ClockCollected();
                    break;
                case ToolType.Helmet:
                    if (HelmetCollected != null) HelmetCollected();
                    break;
                case ToolType.Life:
                    if (LifeCollected != null) LifeCollected();
                    break;
                case ToolType.Power:
                    if (PowerCollected != null) PowerCollected();
                    break;
                case ToolType.Spade:
                    if (SpadeCollected != null) SpadeCollected();
                    break;
            }
            Destroy(other.gameObject);
        }
    }
}
