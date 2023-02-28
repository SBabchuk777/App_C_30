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

        public SlotType SlotType { get; private set; }

        public void SetDefaultSlot(SlotType slot)
        {
            SlotType = slot;

            _slotImage.sprite = SlotType.slotSprite;
        }

        public void SetSlot(SlotType slot, Action onCompleteAnim = null)
        {
            SlotType = slot;

            RectTransform slotTransform = _slotImage.rectTransform;

            slotTransform.DOKill();

            slotTransform.DOScaleX(0f, 0.125f).SetEase(Ease.InSine).OnComplete(() =>
            {
                _slotImage.sprite = SlotType.slotSprite;

                slotTransform.DOScaleX(1f, 0.125f).SetEase(Ease.OutSine).OnComplete(() =>
                    onCompleteAnim?.Invoke());
            });
        }

        public void PlayLineAnim()
        {
            RectTransform slotTransform = _slotImage.rectTransform;

            slotTransform.DOKill();

            slotTransform.DOScale(1.1f, 0.25f).OnComplete(() =>
            {
                slotTransform.DOScale(1f, 0.25f).OnComplete(() =>
                {
                    slotTransform.DOScale(1.1f, 0.25f).OnComplete(() =>
                    {
                        slotTransform.DOScale(1f, 0.25f).OnComplete(() =>
                        {
                        });
                    });
                });
            });
        }
    }
}