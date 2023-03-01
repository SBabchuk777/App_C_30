using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UI.Panels;
using UnityEngine;
using UnityEngine.UI;


namespace Level
{
    public class BetSelector : MonoBehaviour
    {
        [SerializeField] private int _startBet = 25;

        [Space]

        [SerializeField] private Text _betText = null;

        [Space]

        [SerializeField] private Button _addButton = null;

        [Space]

        [SerializeField] private Button _minusButton = null;

        [Space]

        [SerializeField] private int[] _betSteps = new int[0];

        private int _currentBet = 1;

        private Tween _counterTween = null;

        public int CurrentBet
        {
            get => _currentBet;

            private set
            {
                _currentBet = value;

                CheckBet();

                if (_counterTween != null && _counterTween.IsActive())
                    _counterTween.Kill();

                _counterTween = DOVirtual.Int(int.Parse(_betText.text), _currentBet, 0.35f,
                    (value) => _betText.text = value.ToString());
            }
        }

        public int CurrentStepIndex
        {
            get
            {
                int currentStep = _betSteps.Where(s => s <= CurrentBet).Last();

                return Array.IndexOf(_betSteps, currentStep);
            }
        }

        private void Awake()
        {
            CurrentBet = _startBet;

            _addButton.onClick.AddListener(() =>
            {
                if (CurrentBet != Wallet.Money)
                {
                    if (CurrentBet == _betSteps[CurrentStepIndex] && CurrentStepIndex < _betSteps.Length - 1)
                        CurrentBet = _betSteps[CurrentStepIndex + 1];
                    else
                    {
                        int counts = CurrentBet / _betSteps[CurrentStepIndex] + 1;

                        CurrentBet = _betSteps[CurrentStepIndex] * counts;
                    }
                }
            });

            _minusButton.onClick.AddListener(() =>
            {
                if (CurrentBet > _betSteps[0])
                {
                    if (CurrentBet > _betSteps[CurrentStepIndex])
                    {
                        int additionals = CurrentBet % _betSteps[CurrentStepIndex];

                        if (additionals != 0)
                            CurrentBet -= additionals;
                        else
                            CurrentBet -= _betSteps[CurrentStepIndex];
                    }
                    else
                        CurrentBet = _betSteps[CurrentStepIndex - 1];
                }
            });
        }

        public void SetActive(bool active)
        {
            _addButton.enabled = active;
            _minusButton.enabled = active;
        }

        public void CheckBet()
        {
            if (CurrentBet > Wallet.Money)
                CurrentBet = Wallet.Money;
        }
    }
}