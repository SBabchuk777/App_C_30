using System.Collections.Generic;
using System.Linq;
using Game.Player;
using UnityEngine;

namespace Game.Road.Background
{
    public class BackgroundSpawner : MonoBehaviour
    {
        [SerializeField] private float _speedBoost = 0f;

        [SerializeField] private BackgroundPart _firstRoad = null;
        [SerializeField] private BackgroundPart _backgroundPrefab = null;

        private Transform _targetTransform = null;

        private List<BackgroundPart> _roads = new List<BackgroundPart>();

        private void Awake()
        {
            _targetTransform = FindObjectOfType<Fish>().transform;

            _roads.Add(_firstRoad);
        }

        private void Update()
        {
            float playerPositionX = _targetTransform.position.x;

            transform.position = new Vector3(playerPositionX * _speedBoost, 0f, 0f);

            BackgroundPart frontBackground = _roads.First();

            if (frontBackground.PositionX - playerPositionX < frontBackground.Width / 4f)
            {
                BackgroundPart nextRoad = Instantiate(_backgroundPrefab, transform);

                float nextRoadPosition = frontBackground.PositionX + frontBackground.Width;

                nextRoad.transform.position = new Vector3(nextRoadPosition, -7.5f, 0f);

                _roads.Insert(0, nextRoad);

                if (_roads.Count > 3)
                {
                    BackgroundPart lastRoad = _roads.Last();

                    _roads.Remove(lastRoad);

                    Destroy(lastRoad.gameObject);
                }
            }
        }
    }
}