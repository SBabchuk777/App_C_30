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
        [SerializeField] private Vector2Int _areaSize = new Vector2Int(4, 5);
        [SerializeField] private Slot[] _slots = new Slot[0];

        [Space]
        
        [SerializeField] private SlotType[] _slotTypes = new SlotType[0];

        [Space]

        [SerializeField] private Button _spinButton = null;

        [Space]

        [SerializeField] private BetSelector _betSelector = null;

        private void Awake()
        {
            _spinButton.onClick.AddListener(() =>
                StartCoroutine(Spin()));

            for (int i = 0; i < _slots.Length; i++)
                _slots[i].SetDefaultSlot(GetRandomSlot());
        }

        private IEnumerator Spin()
        {
            _spinButton.interactable = false;
            _betSelector.SetActive(false);

            Slot[,] grid = GetSlotsGrid();
            SlotType[,] newValues = new SlotType[_areaSize.y, _areaSize.x];

            //Create next grid

            for (int y = 0; y < _areaSize.y; y++)
            {
                for (int x = 0; x < _areaSize.x; x++)
                    newValues[y, x] = GetRandomSlot();
            }

            int maxIndex = _areaSize.x + _areaSize.y;

            //Spin anim

            AudioController.PlaySound("slot");

            for (int animIndex = 0; animIndex < maxIndex; animIndex++)
            {
                for (int y = 0; y < _areaSize.y; y++)
                {
                    for (int x = 0; x < _areaSize.x; x++)
                    {
                        float distance = x + y;

                        if (distance == animIndex)
                            grid[y, x].SetSlot(newValues[y, x]);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.25f);

            float scale = 1f;

            List<Slot> winSlots = new List<Slot>();

            //Check lines

            for (int y = 0; y < _areaSize.y; y++)
            {
                SlotType lineSlotType = grid[y, 0].SlotType;

                bool isLine = true;

                for (int x = 1; x < _areaSize.x; x++)
                {
                    if (lineSlotType != grid[y, x].SlotType)
                    {
                        isLine = false;

                        break;
                    }
                }

                if (isLine)
                {
                    scale *= lineSlotType.horizontalLinePrize;

                    for (int x = 0; x < _areaSize.x; x++)
                    {
                        if (!winSlots.Contains(grid[y, x]))
                            winSlots.Add(grid[y, x]);
                    }
                }
            }

            //Check rows

            for (int x = 0; x < _areaSize.x; x++)
            {
                SlotType lineSlotType = grid[0, x].SlotType;

                bool isLine = true;

                for (int y = 1; y < _areaSize.y; y++)
                {
                    if (lineSlotType != grid[y, x].SlotType)
                    {
                        isLine = false;

                        break;
                    }
                }

                if (isLine)
                {
                    scale *= lineSlotType.verticalLinePrize;

                    for (int y = 0; y < _areaSize.y; y++)
                    {
                        if (!winSlots.Contains(grid[y, x]))
                            winSlots.Add(grid[y, x]);
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
            Slot[,] grid = new Slot[_areaSize.y, _areaSize.x];

            int index = 0;

            for (int y = 0; y < _areaSize.y; y++)
            {
                for (int x = 0; x < _areaSize.x; x++)
                {
                    grid[y, x] = _slots[index];

                    index++;
                }
            }

            return grid;
        }

        private SlotType GetRandomSlot()
        {
            float maxValue = SpawnRateTo(null);

            float randomValue = Random.Range(0f, maxValue);

            return _slotTypes.Where(s => randomValue <= SpawnRateTo(s)).First();
        }

        private float SpawnRateTo(SlotType slot)
        {
            float rate = 0f;

            for (int i = 0; i < _slotTypes.Length; i++)
            {
                rate += _slotTypes[i].spawnRate;

                if (_slotTypes[i] == slot)
                    return rate;
            }

            return rate;
        }
    }
}