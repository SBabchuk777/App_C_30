using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace Level.AutomateMachine
{
    public class Automate : MonoBehaviour
    {
        [SerializeField] private SlotType[] _slotTypes = new SlotType[0];

        [Space]

        [SerializeField] private SlotsLine[] _slotLines = new SlotsLine[0];

        [Space]

        [SerializeField] private Button _spinButton = null;
        [SerializeField] private Toggle _autoSpinToggle = null;

        [Space]

        [SerializeField] private BetSelector _betSelector = null;

        private void Awake()
        {
            SlotsRandomizer.InitPool(_slotTypes);

            foreach (SlotsLine line in _slotLines)
                line.Init();

            _spinButton.onClick.AddListener(() =>
                StartCoroutine(Spin()));

            _autoSpinToggle.onValueChanged.AddListener((active) =>
            {
                if (!active)
                    return;

                if (!_spinButton.interactable)
                    return;

                if (_betSelector.CurrentBet == 0)
                {
                    _autoSpinToggle.isOn = false;
                    return;
                }

                StartCoroutine(Spin());
            });
        }

        private IEnumerator Spin()
        {
            if (_betSelector.CurrentBet == 0)
                yield break;

            _spinButton.interactable = false;
            _betSelector.SetActive(false);

            yield return new WaitForSeconds(0.1f);

            //Spin anim

            AudioController.PlaySound("slot");

            foreach (SlotsLine line in _slotLines)
            {
                line.StartSpin();

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.75f);

            Slot[,] grid = GetSlotsGrid();

            List<Slot> winSlots = new List<Slot>();

            float scale = BonusCalculator.GetBonus(grid, winSlots);

            //Results

            if (scale != 1)
            {
                AudioController.PlaySound("win");

                for (int i = 0; i < winSlots.Count; i++)
                    winSlots[i].PlayLineAnim();

                float winUnrounded = _betSelector.CurrentBet * (scale - 1);
                int win = (int)winUnrounded;

                if (winUnrounded % 1 > 0)
                    win++;

                Wallet.AddMoney(win);

                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                AudioController.PlaySound("loose");

                Wallet.TryPurchase(_betSelector.CurrentBet);

                _betSelector.CheckBet();

                yield return new WaitForSeconds(0.25f);
            }

            if (_autoSpinToggle.isOn && _betSelector.CurrentBet != 0)
            {
                yield return new WaitForSeconds(0.25f);

                StartCoroutine(Spin());
            }
            else
            {
                _autoSpinToggle.isOn = false;

                _spinButton.interactable = true;
                _betSelector.SetActive(true);
            }
        }

        private Slot[,] GetSlotsGrid()
        {
            int width = _slotLines.Count();
            int height = _slotLines.First().TargetSlots.Count();

            Slot[,] grid = new Slot[width, height];

            for (int x = 0; x < width; x++)
            {
                Slot[] line = _slotLines[x].TargetSlots;

                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = line[y];
                }
            }

            return grid;
        }
    }
}