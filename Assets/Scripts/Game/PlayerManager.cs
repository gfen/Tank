using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Game
{
    public class PlayerManager : NetworkBehaviour
    {
        public PlayerData[] PlayerDatas;

        public GameObject Player;

        public GameObject TankPlaceholder;

        public event Action AllPlayersDestroyed;

        private int _actualPlayerNumber;

        private int _allLifeNumber;

        private int[] _lifeNumbers;

        private int[] _destroyNumbers;

        private int[] _powers;

        private GameObject[] _spawnedPlayers;

        protected static PlayerManager _instance = null;

        public static PlayerManager Instance
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
            _actualPlayerNumber = PlayerInfo.PlayerInfos.Count;
            _destroyNumbers = new int[_actualPlayerNumber];
            _powers = new int[_actualPlayerNumber];
            for (int i = 0; i < _actualPlayerNumber; i++)
            {
                SetPower(i, 1);
            }
            _allLifeNumber = 0;
            _lifeNumbers = new int[_actualPlayerNumber];
            for (int i = 0; i < _actualPlayerNumber; i++)
            {
                SetLifeNumber(i, PlayerDatas[i].LifeNumber);
                _allLifeNumber += _lifeNumbers[i];
            }
            _spawnedPlayers = new GameObject[_actualPlayerNumber];
        }

        public void Spawn()
        {
            for (int i = 0; i < _actualPlayerNumber; i++)
            {
                SetLifeNumber(i, _lifeNumbers[i]);
                SetDestroyNumber(i, 0);
                SetPower(i, _powers[i]);
                StartCoroutine(SpawnPlayer(i));
            }
        }

        private IEnumerator SpawnPlayer(int playerIndex)
        {
            if (_lifeNumbers[playerIndex] <= 0) yield break;

            var placeholder = (GameObject)Instantiate(TankPlaceholder, PlayerDatas[playerIndex].SpawnPoint.position, PlayerDatas[playerIndex].SpawnPoint.rotation);
            NetworkServer.Spawn(placeholder);

            yield return new WaitForSeconds(1f);

            Destroy(placeholder);

            var player = SpawnHelper.Spawn(Player, PlayerDatas[playerIndex].SpawnPoint, transform);
            var playerController = player.GetComponent<PlayerController>();
            playerController.ApplyPlayerInfo(PlayerInfo.PlayerInfos[playerIndex]);
            NetworkServer.SpawnWithClientAuthority(player, PlayerInfo.PlayerInfos[playerIndex].connectionToClient);

            _spawnedPlayers[playerIndex] = player;

            var tankLife = player.GetComponent<TankLife>();
            tankLife.Destroyed += (position) => OnPlayerDestroyed(playerIndex);

            var tankEmitter = player.GetComponent<TankEmitter>();
            tankEmitter.BulletPower = _powers[playerIndex];
            tankEmitter.BulletHit += () => OnBulletHit(playerIndex);

            var cameraSwitcher = player.GetComponent<CameraSwitcher>();
            cameraSwitcher.CameraSwitched += (switchedCamera) => OnCameraSwitched(playerIndex, switchedCamera);

            var toolCollector = player.GetComponent<TankPlayerToolCollector>();
            toolCollector.AntitankGrenadeCollected += () => OnAntitankGrenadeCollected(playerIndex);
            toolCollector.ClockCollected += OnClockCollected;
            toolCollector.HelmetCollected += () => OnHelmetCollected(playerIndex);
            toolCollector.LifeCollected += () => OnLifeCollected(playerIndex);
            toolCollector.PowerCollected += () => OnPowerCollected(playerIndex);
            toolCollector.SpadeCollected += OnSpadeCollected;
        }

        private void OnPlayerDestroyed(int playerIndex)
        {
            SetLifeNumber(playerIndex, _lifeNumbers[playerIndex] - 1);
            SetPower(playerIndex, 1);
            _allLifeNumber--;
            if (_allLifeNumber <= 0)
            {
                if (AllPlayersDestroyed != null) AllPlayersDestroyed();
            }
            StartCoroutine(SpawnPlayer(playerIndex));
        }

        private void OnBulletHit(int playerIndex)
        {
            SetDestroyNumber(playerIndex, _destroyNumbers[playerIndex] + 1);
        }

        private void OnCameraSwitched(int playerIndex, Camera switchedCamera)
        {
            switchedCamera.rect = new Rect(1f/_actualPlayerNumber*playerIndex, 0f, 1f / _actualPlayerNumber, 1f);
            var tankPlayerMovement = _spawnedPlayers[playerIndex].GetComponent<TankPlayerMovement>();
            tankPlayerMovement.SwitchFirstPerson();
        }

        private void OnAntitankGrenadeCollected(int playerIndex)
        {
            var count = EnemyManager.Instance.DestroyActiveEnemies();
            SetDestroyNumber(playerIndex, _destroyNumbers[playerIndex] + count);
        }

        private void OnClockCollected()
        {
            EnemyManager.Instance.SuspendEnemies(20f);
        }

        private void OnHelmetCollected(int playerIndex)
        {
            var tankLife = _spawnedPlayers[playerIndex].GetComponent<TankLife>();
            tankLife.ChangeToInvincibility(20f);
        }

        private void OnLifeCollected(int playerIndex)
        {
            SetLifeNumber(playerIndex, _lifeNumbers[playerIndex] + 1);
            _allLifeNumber++;
        }

        private void OnPowerCollected(int playerIndex)
        {
            var tankEmitter = _spawnedPlayers[playerIndex].GetComponent<TankEmitter>();
            tankEmitter.AddBulletPower();
            SetPower(playerIndex, tankEmitter.BulletPower);
        }

        private void OnSpadeCollected()
        {
            MapManager.Instance.SetSteelsSurroundHome(20f);
        }

        private void SetLifeNumber(int playerIndex, int number)
        {
            _lifeNumbers[playerIndex] = number;
            PlayerDatas[playerIndex].LifeNumberText.text = number.ToString();

            RpcSetLifeNumber(playerIndex, number);
        }

        [ClientRpc]
        private void RpcSetLifeNumber(int playerIndex, int number)
        {
            if (isServer) return;

            PlayerDatas[playerIndex].LifeNumberText.text = number.ToString();
        }

        private void SetDestroyNumber(int playerIndex, int number)
        {
            _destroyNumbers[playerIndex] = number;
            PlayerDatas[playerIndex].DestroyNumberText.text = number.ToString();

            RpcSetDestroyNumber(playerIndex, number);
        }

        [ClientRpc]
        private void RpcSetDestroyNumber(int playerIndex, int number)
        {
            if (isServer) return;

            PlayerDatas[playerIndex].DestroyNumberText.text = number.ToString();
        }

        private void SetPower(int playerIndex, int power)
        {
            _powers[playerIndex] = power;
            PlayerDatas[playerIndex].PowerText.text = power.ToString();

            RpcSetPower(playerIndex, power);
        }

        [ClientRpc]
        private void RpcSetPower(int playerIndex, int power)
        {
            if (isServer) return;

            PlayerDatas[playerIndex].PowerText.text = power.ToString();
        }

        public void Clear()
        {
            SpawnHelper.ClearAll(transform);
        }
    }

    [Serializable]
    public class PlayerData
    {
        public Transform SpawnPoint;

        public int LifeNumber;

        public Text LifeNumberText;

        public Text DestroyNumberText;

        public Text PowerText;
    }
}
