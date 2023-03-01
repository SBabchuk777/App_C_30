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

        [Space]

        [SerializeField] private BetSelector _betSelector = null;

        private void Awake()
        {
            SlotsRandomizer.InitPool(_slotTypes);

            foreach (SlotsLine line in _slotLines)
                line.Init();

            _spinButton.onClick.AddListener(() =>
                StartCoroutine(Spin()));
        }

        private IEnumerator Spin()
        {
            if (_betSelector.CurrentBet == 0)
                yield break;

            _spinButton.interactable = false;
            _betSelector.SetActive(false);

            //Spin anim

            AudioController.PlaySound("slot");

            foreach (SlotsLine line in _slotLines)
            {
                line.StartSpin();

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.75f);

            Slot[,] grid = GetSlotsGrid();

            float scale = 1f;

            List<Slot> winSlots = new List<Slot>();

            //Check rows

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                SlotType lineSlotType = grid[0, y].SlotType;

                bool isLine = true;

                for (int x = 1; x < grid.GetLength(0); x++)
                {
                    if (lineSlotType != grid[x, y].SlotType)
                    {
                        isLine = false;

                        break;
                    }
                }

                if (isLine)
                {
                    scale *= lineSlotType.horizontalLinePrize;

                    for (int x = 0; x < grid.GetLength(0); x++)
                    {
                        if (!winSlots.Contains(grid[x, y]))
                            winSlots.Add(grid[x, y]);
                    }
                }
            }

            //Check columns

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                SlotType lineSlotType = grid[x, 0].SlotType;

                bool isLine = true;

                for (int y = 1; y < grid.GetLength(1); y++)
                {
                    if (lineSlotType != grid[x, y].SlotType)
                    {
                        isLine = false;

                        break;
                    }
                }

                if (isLine)
                {
                    scale *= lineSlotType.verticalLinePrize;

                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        if (!winSlots.Contains(grid[x, y]))
                            winSlots.Add(grid[x, y]);
                    }
                }
            }

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

                yield return new WaitForSeconds(1f);
            }
            else
            {
                AudioController.PlaySound("loose");

                Wallet.TryPurchase(_betSelector.CurrentBet);

                _betSelector.CheckBet();

                yield return new WaitForSeconds(0.25f);
            }

            _spinButton.interactable = true;
            _betSelector.SetActive(true);
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