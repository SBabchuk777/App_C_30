using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace Level.AutomateMachine
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Image _slotImage = null;
        [SerializeField] private Image _glowImage = null;

        private Sequence _lineAnim = null;

        public RectTransform RectTransform { get; private set; }

        public SlotType SlotType { get; private set; }

        private void Awake() =>
            RectTransform = GetComponent<RectTransform>();

        public void SetSlot(SlotType slot)
        {
            SlotType = slot;

            _slotImage.sprite = SlotType.slotSprite;
        }

        public void PlayLineAnim()
        {
            if (_lineAnim != null && _lineAnim.IsActive())
                _lineAnim.Kill();

            _lineAnim.Append(_glowImage.DOFade(0.5f, 0.125f));

            _lineAnim.Append(_glowImage.DOFade(1f, 0.25f)
                .SetLoops(6, LoopType.Yoyo));

            _lineAnim.Append(_glowImage.DOFade(0f, 0.125f));
        }
    }
}