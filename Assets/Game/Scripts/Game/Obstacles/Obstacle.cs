using UnityEngine;

namespace Game.Obstacles
{
    public class Obstacle : MonoBehaviour, IScoreObstacle
    {
        [SerializeField] private Collider2D _collider = null;

        public float GetCenterX() =>
            transform.position.x + _collider.offset.x;

        public float GetSizeX() =>
            _collider.bounds.size.x;
    }
}
