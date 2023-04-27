using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

        private List<Transform> _obstacles = new List<Transform>();

        public int Score { get; private set; }

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
            if (!_tutorial.IsCompleted || _obstacles.Count == 0)
                return;

            if (_obstacles.First().position.x + 2f < _targetTransform.position.x)
            {
                _obstacles.RemoveAt(0);

                Score++;

                UpdateScoreText();

                if (Score > MaxScore)
                    MaxScore = Score;
            }
        }

        private void UpdateScoreText()
        {
            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _text.text = $"Score: {Score.ToString("#,0", format)}";
        }

        public void AddObstacle(Transform obstacle)
        {
            if (!_obstacles.Contains(obstacle))
                _obstacles.Add(obstacle);
        }
    }
}