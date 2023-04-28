using System.Collections.Generic;
using Game.Obstacles;
using Game.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Road
{
    public class ObstacleGenerator : MonoBehaviour
    {
        [Header("Prefabs")]

        [Space]

        [SerializeField] private GameObject[] _obstaclesPrefabs = new GameObject[0];
        [SerializeField] private GameObject[] _enemiesPrefabs = new GameObject[0];
        [SerializeField] private GameObject[] _bonusesPrefabs = new GameObject[0];

        [Header("Spawn Settings")]

        [Space]

        [SerializeField] private float _startSpawnOffset = 30f;
        [SerializeField] private float _destroyOffset = 20f;

        [Space]

        [SerializeField] private float _obstacleMinY = -2f;
        [SerializeField] private float _obstacleMaxY = 2f;

        [Space]

        [Range(0, 1f)][SerializeField] private float _bonusSpawnRate = 0.5f;

        [Space]

        [SerializeField] private float _bonusMinY = -1.5f;
        [SerializeField] private float _bonusMaxY = 1.5f;

        [Space]

        [Range(0, 1f)][SerializeField] private float _fishSpawnRate = 0.25f;
        
        [Space]

        [SerializeField] private float _fishMinY = -2.5f;
        [SerializeField] private float _fishMaxY = 2.5f;

        [Header("Difficult Settings")]

        [Space]

        [SerializeField] private float _scoreForHard = 25f;
        
        [Space]

        [SerializeField] private float _easeDistance = 25f;
        [SerializeField] private float _hardDistance = 10f;

        private Transform _player = null;

        private Tutorial _tutorial = null;
        private ScoreCounter _scoreCounter = null;

        private float _lastSpawnDistance = 10f;

        private List<GameObject> _obstacles = new List<GameObject>();

        private void Start()
        {
            _player = FindObjectOfType<Fish>().transform;

            _tutorial = FindObjectOfType<Tutorial>();
            _scoreCounter = FindObjectOfType<ScoreCounter>();

            _tutorial.OnComplete += () =>
            {
                _lastSpawnDistance = _player.position.x + _startSpawnOffset;
            };
        }

        private void Update()
        {
            if (!_tutorial.IsCompleted)
                return;

            if (_player.position.x + _startSpawnOffset > _lastSpawnDistance)
                SpawnNext();
            
            for (int i = 0; i < _obstacles.Count; i++)
            {
                if (_obstacles[i] != null && _obstacles[i].transform.position.x < _player.position.x - _destroyOffset)
                {
                    Destroy(_obstacles[i]);

                    _obstacles.RemoveAt(i);

                    i--;
                }
            }
        }

        private void SpawnNext()
        {
            //Spawn Obstacle
            
            float distance = Mathf.Lerp(_easeDistance, _hardDistance, _scoreCounter.TotalScore / _scoreForHard);

            float currentSpawnDistance = _lastSpawnDistance + distance;
            
            float randomY = Random.Range(_obstacleMinY, _obstacleMaxY);

            Vector3 obstacleSpawnPosition = new Vector3(currentSpawnDistance, randomY, 0f);

            GameObject obstaclePrefab = _obstaclesPrefabs[Random.Range(0, _obstaclesPrefabs.Length)];
            
            GameObject obstacle = Instantiate(obstaclePrefab, obstacleSpawnPosition, Quaternion.identity, transform);

            _obstacles.Add(obstacle);

            _scoreCounter.AddObstacle(obstacle.GetComponent<IScoreObstacle>());

            //Spawn Bonus
            
            if (Random.value > _bonusSpawnRate)
            {
                GameObject bonusPrefab = _bonusesPrefabs[Random.Range(0, _bonusesPrefabs.Length)];
                
                Vector3 bonusSpawnPosition = obstacleSpawnPosition;

                bonusSpawnPosition.x = (_lastSpawnDistance + currentSpawnDistance) / 2f;
                bonusSpawnPosition.y = Random.Range(_bonusMinY, _bonusMaxY);

                Instantiate(bonusPrefab, bonusSpawnPosition, Quaternion.identity, transform);
            }
            
            //Spawn Enemy
            
            if (Random.value > _fishSpawnRate)
            {
                GameObject enemyPrefab = _enemiesPrefabs[Random.Range(0, _enemiesPrefabs.Length)];

                Vector3 fishSpawnPosition = obstacleSpawnPosition;

                fishSpawnPosition.x = currentSpawnDistance + 2.5f;
                fishSpawnPosition.y = Random.Range(_fishMinY, _fishMaxY);

                GameObject enemy = Instantiate(enemyPrefab, fishSpawnPosition, Quaternion.identity, transform);
                
                _scoreCounter.AddObstacle(enemy.GetComponent<IScoreObstacle>());
            }

            _lastSpawnDistance = currentSpawnDistance;
        }
    }
}