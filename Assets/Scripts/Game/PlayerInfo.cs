using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

namespace Game
{
    public class PlayerInfo : NetworkBehaviour
    {
        public string Name;

        public Color Color;

        private static List<PlayerInfo> _playerInfos = new List<PlayerInfo>();
        public static List<PlayerInfo> PlayerInfos { get { return _playerInfos; } }
        
        public static void AddPlayer(PlayerInfo playerInfo)
        {
            _playerInfos.Add(playerInfo);
        }

        public static bool CheckValid()
        {
            return _playerInfos.Count == LobbyManager.s_Singleton._playerNumber;
        }
    }
}
