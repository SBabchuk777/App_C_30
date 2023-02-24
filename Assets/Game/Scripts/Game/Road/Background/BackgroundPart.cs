using UnityEngine;

namespace Game.Road.Background
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundPart : MonoBehaviour
    {
        private SpriteRenderer _roadRenderer = null;

        public float Width => _roadRenderer.bounds.size.x;

        public float PositionX => transform.position.x;

        private void Awake() =>
            _roadRenderer = GetComponent<SpriteRenderer>();

    }
}