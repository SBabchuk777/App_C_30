using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Level.AutomateMachine;
using UnityEngine;

namespace Level.AutomateMachine
{
    public static class SlotsRandomizer
    {
        private static SlotType[] _slotTypes = null;

        public static void InitPool(SlotType[] slotTypes) =>
            _slotTypes = slotTypes;


        public static SlotType GetRandomSlot()
        {
            float maxValue = SpawnRateTo(null);

            float randomValue = Random.Range(0f, maxValue);

            return _slotTypes.Where(s => randomValue <= SpawnRateTo(s)).First();
        }

        private static float SpawnRateTo(SlotType slot)
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