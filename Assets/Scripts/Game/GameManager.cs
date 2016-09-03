using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : NetworkBehaviour
    {        
        public HomeController HomeController;

        public Text GameOverText;

        public Text WinText;

        public float DefaultNextLevelWaitTime = 3f;

        private bool _ready;

        private bool _nextLevel;

        private bool _gameOver;

        private float _nextLevelWaitTime;

        private bool _pause = false;

        public override void OnStartClient()
        {
            if (isServer) return;

            SetUI();
        }

        public override void OnStartServer()
        {
            SetUI();
            
            PlayerManager.Instance.AllPlayersDestroyed += () => _gameOver = true;
            EnemyManager.Instance.AllEnemiesDestroyed += () => _nextLevel = true;
            HomeController.Destroyed += () => _gameOver = true;

            _ready = false;
            StartCoroutine(Init());         
        }

        private void SetUI()
        {
            GameOverText.enabled = false;
            WinText.enabled = false;

            _nextLevel = true;
            _gameOver = false;
            _nextLevelWaitTime = 0.1f;
        }

        private IEnumerator Init()
        {
            yield return new WaitUntil(PlayerInfo.CheckValid);

            MapManager.Instance.Init();
            PlayerManager.Instance.Init();
            _ready = true;
        }
        
        void Update()
        {
            if (!_ready) return;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (_pause) Time.timeScale = 1f;
                else Time.timeScale = 0f;
                _pause = !_pause;
            }

            if (_gameOver)
            {
                if (!GameOverText.enabled) RpcSetGameOver();
                GameOverText.enabled = true;
                
                return;
            }

            if (_nextLevel)
            {
                if (!MapManager.Instance.MoveNext())
                {
                    if (!WinText.enabled) RpcSetWin();
                    WinText.enabled = true;

                    return;
                }

                StartCoroutine(NextLevel());
            }
        }

        [ClientRpc]
        private void RpcSetGameOver()
        {
            if (isServer) return;

            GameOverText.enabled = true;
        }

        [ClientRpc]
        private void RpcSetWin()
        {
            if (isServer) return;

            WinText.enabled = true;
        }

        private IEnumerator NextLevel()
        {
            _nextLevel = false;

            yield return new WaitForSeconds(_nextLevelWaitTime);

            _nextLevelWaitTime = DefaultNextLevelWaitTime;

            ClearAll();
            
            yield return MapManager.Instance.Load();

            PlayerManager.Instance.Spawn();

            yield return null;

            EnemyManager.Instance.Spawn(MapManager.Instance.GetProgress());

            yield return null;
        }

        private void ClearAll()
        {
            MapManager.Instance.Reset();
            PlayerManager.Instance.Clear();
            BulletManager.Instance.Clear();
            ToolManager.Instance.Clear();
        }

        public void OnExitButtonClick()
        {
            if (_pause) Time.timeScale = 1f;

            if (NetworkManager.singleton != null)
            {
                if (NetworkServer.active || NetworkManager.singleton.IsClientConnected())
                {
                    var networkManagerGameobject = NetworkManager.singleton.gameObject;
                    NetworkManager.singleton.offlineScene = "";
                    NetworkManager.Shutdown();
                    Destroy(networkManagerGameobject);                
                }
            }

            SceneManager.LoadScene("MainMenu");
        }
    }
}
