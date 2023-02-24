using System.Collections.Generic;
using System.Linq;
using Game.Player;
using UnityEngine;

namespace Game.Road.Background
{
    public class BackgroundSpawner : MonoBehaviour
    {
        [SerializeField] private BackgroundPart _firstRoad = null;
        [SerializeField] private BackgroundPart _backgroundPrefab = null;

        private Transform _birdTransform = null;

        private List<BackgroundPart> _roads = new List<BackgroundPart>();

        private void Awake()
        {
            _birdTransform = FindObjectOfType<Bird>().transform;

            _roads.Add(_firstRoad);
        }

        private void Update()
        {
            float playerPositionX = _birdTransform.position.x;

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