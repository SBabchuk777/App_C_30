using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Level.AutomateMachine
{
    public class SlotsLine : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _accelerationCurve = null;

        [Space]

        [SerializeField] private float _time = 0.5f;
        [SerializeField] private float _itemHeight = 90f;

        [Space]

        [SerializeField] private int _minSpinCount = 10;
        [SerializeField] private int _maxSpinCount = 15;

        [Space]

        [SerializeField] private List<Slot> _slotsLine = new List<Slot>();

        private SlotType[] _slotTypes = null;

        public Slot[] TargetSlots => new Slot[] { _slotsLine[1], _slotsLine[2], _slotsLine[3] };

        public void Init()
        {
            foreach (Slot slot in _slotsLine)
                slot.SetSlot(SlotsRandomizer.GetRandomSlot());
        }

        public void StartSpin()
        {
            float lastOffset = 0f;

            float randomOffset = _itemHeight * Random.Range(_minSpinCount, _maxSpinCount);

            float lineHeight = _itemHeight * _slotsLine.Count;
            float minVertical = -(lineHeight / 2f);

            DOVirtual.Float(0, randomOffset, _time, (offset) =>
            {
                float acceleration = offset - lastOffset;

                lastOffset = offset;

                List<Slot> slotsForMove = new List<Slot>();

                for (int i = _slotsLine.Count() - 1; i >= 0; i--)
                {
                    Slot slot = _slotsLine[i];

                    RectTransform rectTransform = slot.RectTransform;

                    rectTransform.anchoredPosition += Vector2.down * acceleration;

                    if (rectTransform.anchoredPosition.y <= minVertical)
                        slotsForMove.Add(slot);
                }

                foreach (Slot slot in slotsForMove)
                {
                    RectTransform rectTransform = slot.RectTransform;

                    rectTransform.anchoredPosition += Vector2.up * lineHeight;

                    slot.SetSlot(SlotsRandomizer.GetRandomSlot());

                    _slotsLine.Remove(slot);
                    _slotsLine.Insert(0, slot);

                    rectTransform.SetSiblingIndex(0);
                }
            });
        }
    }
}