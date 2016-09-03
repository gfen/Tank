using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class PlayerController : NetworkBehaviour
    {
        [SyncVar]
        public string Name = "";
        [SyncVar]
        public Color Color = Color.white;
        
        public void ApplyPlayerInfo(PlayerInfo playerInfo)
        {
            Name = playerInfo.Name;
            Color = playerInfo.Color;

            ApplyInfo();
        }

        public override void OnStartClient()
        {
            ApplyInfo();
        }
        
        private void ApplyInfo()
        {
            var renderer = transform.Find("Model/Main").GetComponent<Renderer>();
            renderer.material.color = Color;
        }
    }
}
