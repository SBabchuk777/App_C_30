using DG.Tweening;
using UnityEngine;

namespace Game.Player
{
    public class Camera : MonoBehaviour
    {
        private Transform _birdTransform = null;

        private float _lastOffset = 0f;
        private float _currentOffset = 0f;

        private Tutorial _tutorial;

        private Tween _smoothTween = null;

        private void Awake()
        {
            _birdTransform = FindObjectOfType<Bird>().transform;

            _tutorial = FindObjectOfType<Tutorial>();

            _tutorial.OnComplete += () =>
            {
                float offset = 7.5f + (UnityEngine.Camera.main.aspect - 0.2f);

                SetOffset(offset);
            };
        }

        private void LateUpdate()
        {
            Vector3 position = transform.position;

            position.x = _birdTransform.position.x + _currentOffset;

            transform.position = position;
        }

        public void SetOffset(float offset)
        {
            if (_smoothTween != null && _smoothTween.IsActive())
                _smoothTween.Kill();

            _smoothTween = DOVirtual.Float(_currentOffset, offset, Mathf.Abs(offset - _lastOffset) / 5f,
                (smoothedOffset) => _currentOffset = smoothedOffset);

            _lastOffset = offset;
        }
    }
}