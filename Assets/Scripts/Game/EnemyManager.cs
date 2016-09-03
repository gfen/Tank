using System;
using UnityEngine;
using System.Collections;
using Framework;
using UnityEngine.Networking;

namespace Game
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        public GameObject[] EnemyPrefabs;

        public Transform[] EnemySpawnPoints;

        public int Number;

        public GameObject TankPlaceholder;

        public event Action AllEnemiesDestroyed;

        private int _enemyNumber;

        private GameObject[] _spawnedEnemies;

        private bool _suspended = false;

        private Coroutine _suspendCoroutine;

        void Awake()
        {
            _instance = this;

            _spawnedEnemies = new GameObject[Number];
        }

        public void Spawn(float progress)
        {
            _suspended = false;
            StartCoroutine(SpawnCore(progress));
        }

        private IEnumerator SpawnCore(float progress)
        {
            _enemyNumber = Number;

            var enemySequence = CreateEnemySequence(progress);

            for (int i = 0; i < Number; i++)
            {
                yield return new WaitForSeconds(2f);

                var placeholder = (GameObject)Instantiate(TankPlaceholder, EnemySpawnPoints[i % EnemySpawnPoints.Length].position, EnemySpawnPoints[i % EnemySpawnPoints.Length].rotation);
                NetworkServer.Spawn(placeholder);

                yield return new WaitForSeconds(1f);

                Destroy(placeholder);

                var enemy = SpawnHelper.Spawn(EnemyPrefabs[enemySequence[i]], EnemySpawnPoints[i%EnemySpawnPoints.Length], transform);
                NetworkServer.Spawn(enemy);

                _spawnedEnemies[i] = enemy;

                var tankLife = enemy.GetComponent<TankLife>();
                tankLife.Destroyed += OnEnemyDestroyed;

                if (_suspended)
                {
                    SuspendEnemy(enemy);
                }
            }
        }

        private int[] CreateEnemySequence(float progress)
        {
            var stageNumber = EnemyPrefabs.Length - 1f;
            var stage = 0;
            if (EnemyPrefabs.Length > 1)
            {
                stage = 1;
                while (progress > stage/stageNumber + float.Epsilon) stage++;
            }
            
            var numbers = new int[EnemyPrefabs.Length];
            var previousNumber = Mathf.FloorToInt((-stageNumber/(stage*(stage + 1f))*progress + 2f/(stage + 1) + float.Epsilon)*Number);
            var previousTotalNumber = 0;
            for (int i = 0; i < stage; i++)
            {
                numbers[i] = previousNumber;
                previousTotalNumber += numbers[i];
            }
            numbers[stage] = Number - previousTotalNumber;
            for (int i = stage + 1; i < EnemyPrefabs.Length; i++)
            {
                numbers[i] = 0;
            }
            
            return CreateSequence(numbers);
        }

        private int[] CreateSequence(int[] numbers)
        {
            var sequence = new int[Number];
            var sequenceIndex = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                var number = numbers[i];
                while (number > 0)
                {
                    sequence[sequenceIndex] = i;
                    sequenceIndex++;
                    number--;
                }
            }

            return sequence;
        }

        private void OnEnemyDestroyed(Vector3 position)
        {
            _enemyNumber--;
            if (_enemyNumber <= 0)
            {
                if (AllEnemiesDestroyed != null) AllEnemiesDestroyed();
            }
            ToolManager.Instance.SpawnRandomly(position);
        }

        public int DestroyActiveEnemies()
        {
            var count = 0;

            for (int i = 0; i < _spawnedEnemies.Length; i++)
            {
                if (_spawnedEnemies[i] != null)
                {
                    Destroy(_spawnedEnemies[i]);
                    OnEnemyDestroyed(_spawnedEnemies[i].transform.position);
                    count++;
                }
            }

            return count;
        }

        public void SuspendEnemies(float durationTime)
        {
            if (_suspended) StopCoroutine(_suspendCoroutine);

            _suspended = true;

            for (int i = 0; i < _spawnedEnemies.Length; i++)
            {
                if (_spawnedEnemies[i] != null)
                {
                    SuspendEnemy(_spawnedEnemies[i]);
                }
            }

            _suspendCoroutine = StartCoroutine(SuspendCore(durationTime));
        }

        private IEnumerator SuspendCore(float durationTime)
        {
            yield return new WaitForSeconds(durationTime);

            _suspended = false;

            for (int i = 0; i < _spawnedEnemies.Length; i++)
            {
                if (_spawnedEnemies[i] != null)
                {
                    ResumeEnemy(_spawnedEnemies[i]);
                }
            }
        }

        private void SuspendEnemy(GameObject enemy)
        {
            var tankEnemyMovement = enemy.GetComponent<TankEnemyMovement>();
            tankEnemyMovement.Suspend();

            var tankEnemyEmitter = enemy.GetComponent<TankEnemyEmitter>();
            tankEnemyEmitter.Suspend();
        }

        private void ResumeEnemy(GameObject enemy)
        {
            var tankEnemyMovement = enemy.GetComponent<TankEnemyMovement>();
            tankEnemyMovement.Resume();

            var tankEnemyEmitter = enemy.GetComponent<TankEnemyEmitter>();
            tankEnemyEmitter.Resume();
        }
    }
}
