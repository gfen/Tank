using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Game
{
    public class MapManager : NetworkBehaviour
    {
        public GameObject MapGameObject;

        public Text LevelText;

        private string[] _mapFilenames;

        private int _currentMapIndex;

        private Map _map;

        protected static MapManager _instance = null;

        public static MapManager Instance
        {
            get
            {
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        public void Init()
        {
            _mapFilenames = MapFileManager.Instance.GetAllMapFilenames();
            _currentMapIndex = -1;
            _map = new Map(MapGameObject);
            _map.ObstacleSpawned += NetworkServer.Spawn;
        }

        public bool MoveNext()
        {
            _currentMapIndex++;
            if (_currentMapIndex >= _mapFilenames.Length) return false;
            LevelText.text = _mapFilenames[_currentMapIndex];
            RpcSetLevel(_mapFilenames[_currentMapIndex]);
            return true;
        }

        [ClientRpc]
        private void RpcSetLevel(string level)
        {
            if (isServer) return;

            LevelText.text = level;
        }

        public float GetProgress()
        {
            return (float)_currentMapIndex/(_mapFilenames.Length);
        }

        public void Reset()
        {
            _map.Reset();
        }

        public IEnumerator Load()
        {
            yield return null;

            var path = MapFileManager.Instance.GetPath(_mapFilenames[_currentMapIndex]);

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                _map.Import(fileStream);
            }

            yield return null;

            _map.Build();

            yield return null;
        }

        public void SetSteelsSurroundHome(float durationTime)
        {
            _map.ClearSurroundHome();
            _map.BuildSurroundHome(ObstacleType.Steel, obstacle =>
            {
                var disappearingObject = obstacle.AddComponent<DisappearingObject>();
                disappearingObject.DisappearTime = durationTime;
                var position = obstacle.transform.position;
                disappearingObject.Disappeared += () => RebuildSurroundHome(position);
            });
        }

        private void RebuildSurroundHome(Vector3 position)
        {
            _map.InstantiateObstacle(ObstacleType.Brick, position);
        }
    }
}
