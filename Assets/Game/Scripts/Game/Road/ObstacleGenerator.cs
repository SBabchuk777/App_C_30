using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

namespace Game.Road
{
    public class ObstacleGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _obstaclePrefab = null;
        [SerializeField] private GameObject[] _bonuses = new GameObject[0];

        [Space]

        [SerializeField] private float _minY = -3f;
        [SerializeField] private float _maxY = 3f;

        [Space]

        [SerializeField] private float _minBonusY = -1.5f;
        [SerializeField] private float _maxBonusY = 1.5f;

        [Space]

        [SerializeField] private float _easeDistance = 25f;
        [SerializeField] private float _hardDistance = 10f;

        [Space]

        [SerializeField] private float _startSpawnOffset = 30f;
        [SerializeField] private float _destroyOffset = 20f;

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
                SpawnObstacle();
            
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

        private void SpawnObstacle()
        {
            float randomY = Random.Range(_minY, _maxY);

            Vector3 spawnPosition = new Vector3(_lastSpawnDistance, randomY, 0f);

            GameObject obstacle = Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity, transform);

            _obstacles.Add(obstacle);

            _scoreCounter.AddObstacle(obstacle.transform);

            float distance = Mathf.Lerp(_easeDistance, _hardDistance, _scoreCounter.Score / 50f);

            _lastSpawnDistance += distance;

            if (Random.value > 0.75f)
            {
                spawnPosition.y += Random.Range(_minBonusY, _maxBonusY);

                GameObject bonus = Instantiate(_bonuses[Random.Range(0, _bonuses.Length)], spawnPosition, Quaternion.identity, obstacle.transform);
            }
        }
    }
}