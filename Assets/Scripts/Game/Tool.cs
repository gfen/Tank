using UnityEngine;

namespace Game
{
    public class Tool : MonoBehaviour
    {
        public ToolType Type;
    }

    public enum ToolType
    {
        AntitankGrenade = 0,
        Clock = 1,
        Helmet = 2,
        Life = 3,
        Power = 4,
        Spade = 5,
    }
}
