using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Player
{
    public class Tutorial : MonoBehaviour
    {
        public event Action OnComplete = null;

        [SerializeField] private Transform _finger = null;

        public bool IsCompleted { get; private set; }

        private void Update()
        {
            if (!IsCompleted && Input.GetMouseButtonDown(0))
            {
                _finger.DOScale(0f, 0.25f);

                IsCompleted = true;

                OnComplete?.Invoke();
            }
        }
    }
}