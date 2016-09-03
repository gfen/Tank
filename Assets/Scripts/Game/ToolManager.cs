using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Framework;

namespace Game
{
    public class ToolManager : MonoSingleton<ToolManager>
    {
        public float SpawnPercent;

        public GameObject[] Tools;

        void Awake()
        {
            _instance = this;
        }

        public void SpawnRandomly(Vector3 position)
        {
            if (Random.value < SpawnPercent)
            {
                var tool = (GameObject)Instantiate(Tools[Random.Range(0, Tools.Length)], position, Quaternion.identity);
                tool.transform.parent = transform;

                NetworkServer.Spawn(tool);
            }
        }

        public void Clear()
        {
            SpawnHelper.ClearAll(transform);
        }
    }
}
