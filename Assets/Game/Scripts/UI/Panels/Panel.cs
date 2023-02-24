using DG.Tweening;
using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Panels
{
    public class Panel : MonoBehaviour
    {
        [SerializeField] protected RectTransform _panel = null;

        [Space]

        [SerializeField] protected UnityEvent _onShow = null;
        [SerializeField] protected UnityEvent _onHide = null;

        protected bool _isAniming = false;

        public virtual void Show()
        {
            if (_isAniming)
                return;

            AudioController.PlaySound("wzuuuh");

            DOVirtual.DelayedCall(0.1f, () =>
            {
                _onShow?.Invoke();
            });

            _isAniming = true;

            _panel.localScale = Vector3.zero;

            gameObject.SetActive(true);

            _panel.DOScale(1f, 0.25f).OnComplete(() =>
            {
                _isAniming = false;
            });
        }

        public virtual void Hide()
        {
            if (_isAniming)
                return;

            _isAniming = true;

            _panel.localScale = Vector3.one;

            _panel.DOScale(0f, 0.25f).OnComplete(() =>
            {
                gameObject.SetActive(false);

                _isAniming = false;

                _onHide?.Invoke();
            });
        }
    }
}