using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Level.AutomateMachine
{
    public static class BonusCalculator
    {
        public static float GetBonus(Slot[,] grid, List<Slot> winSlots)
        {
            float scale = 1f;

            scale *= CheckRows(grid, winSlots);

            scale *= CheckColumns(grid, winSlots);
            
            return scale;
        }

        private static float CheckRows(Slot[,] grid, List<Slot> winSlots)
        {
            float scale = 1f;

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

            return scale;
        }

        private static float CheckColumns(Slot[,] grid, List<Slot> winSlots)
        {
            float scale = 1f;

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

            return scale;
        }
    }
}