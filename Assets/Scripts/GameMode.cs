using UnityEngine;
using UnityEngine.Networking;
using Game;

public class GameMode : MonoBehaviour
{
    private static string Source = "";

    private static bool Single = true;
    
    public static void SetSingle(bool single, string source)
    {
        Single = single;
        Source = source;
    }

    void Start()
    {
        PlayerInfo.PlayerInfos.Clear();
        if (Single && Source == "MainMenu")
        {
            NetworkManager.singleton.StartHost();
        }
        Source = "";
    }
}
