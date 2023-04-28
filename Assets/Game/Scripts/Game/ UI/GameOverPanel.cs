using DG.Tweening;
using Game.Player;
using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        public static bool IsGameOver { get; private set; }

        [SerializeField] private Image _fade = null;
        [SerializeField] private Transform _panel = null;

        [Space]

        [SerializeField] private Text _yourScore = null;
        [SerializeField] private Text _bestScore = null;

        [Space]

        [SerializeField] private ScoreCounter _score = null;

        private Fish _fish = null;

        private void Awake()
        {
            IsGameOver = false;

            _fish = FindObjectOfType<Fish>();

            _fish.OnDead += () => DOVirtual.DelayedCall(1f, Show);

            _fade.color = new Color(0f, 0f, 0f, 0f);
            _panel.localScale = Vector3.zero;

            gameObject.SetActive(false);
        }

        private void OnDestroy() =>
            IsGameOver = false;

        public void Show()
        {
            IsGameOver = true;

            DOVirtual.DelayedCall(0.25f, () =>
            {
                _yourScore.text = _score.TotalScore.ToString();
                _bestScore.text = ScoreCounter.MaxScore.ToString();

                gameObject.SetActive(true);

                _fade.DOFade(0.5f, 0.25f).SetEase(Ease.InSine);
                _panel.DOScale(1f, 0.25f).SetEase(Ease.InSine);
            });
        }
    }
}