using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Obstacles;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Game.Player
{
    public class ScoreCounter : MonoBehaviour
    {
        public static int MaxScore
        {
            get => PlayerPrefs.GetInt("MaxScore", 0);

            private set
            {
                PlayerPrefs.SetInt("MaxScore", value);
                PlayerPrefs.Save();
            }
        }

        [SerializeField] private Text _text = null;

        private Transform _targetTransform = null;

        private Tutorial _tutorial = null;

        private float _startDistance = 0f;

        private int _obstaclesTotalScoreBonus = 0;

        private List<IScoreObstacle> _obstacles = new List<IScoreObstacle>();

        private int DistanceScore => Mathf.RoundToInt(_targetTransform.position.x - _startDistance);

        public int TotalScore { get; private set; }

        private void Awake()
        {
            _targetTransform = FindObjectOfType<Fish>().transform;

            _tutorial = FindObjectOfType<Tutorial>();

            _tutorial.OnComplete += () =>
                _startDistance = _targetTransform.position.x;

            UpdateScoreText();
        }

        private void Update()
        {
            if (!_tutorial.IsCompleted || _obstacles.Count == 0 || GameOverPanel.IsGameOver)
                return;

            for (int i = 0; i < _obstacles.Count; i++)
            {
                if ((_obstacles[i] as Object) == null)
                    continue;

                float obstacleEndPosition = _obstacles[i].GetCenterX();
                obstacleEndPosition += (_obstacles[i].GetSizeX() / 2f) + 1f;
                
                if (obstacleEndPosition < _targetTransform.position.x)
                {
                    _obstacles.RemoveAt(0);

                    _obstaclesTotalScoreBonus += 10;
                }
            }

            int newScore = DistanceScore + _obstaclesTotalScoreBonus;

            if (TotalScore < newScore)
            {
                TotalScore = newScore;

                if (TotalScore > MaxScore)
                    MaxScore = TotalScore;
                
                UpdateScoreText();
            }
        }

        private void UpdateScoreText()
        {
            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _text.text = $"Score: {TotalScore.ToString("#,0", format)}";
        }

        public void AddObstacle(IScoreObstacle obstacle)
        {
            if (!_obstacles.Contains(obstacle))
                _obstacles.Add(obstacle);
        }
    }
}