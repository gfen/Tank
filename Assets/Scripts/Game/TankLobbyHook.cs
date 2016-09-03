using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

namespace Game
{
    public class TankLobbyHook : LobbyHook
    {
        public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
        {
            var lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
            var playerInfo = gamePlayer.GetComponent<PlayerInfo>();

            playerInfo.Name = lobby.playerName;
            playerInfo.Color = lobby.playerColor;

            PlayerInfo.AddPlayer(playerInfo);
        }
    }
}
