using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Level.AutomateMachine
{
    [Serializable]
    public class SlotType
    {
        public Sprite slotSprite = null;

        [Space]

        public float spawnRate = 10f;

        [Space]

        public float horizontalLinePrize = 1.1f;
        public float verticalLinePrize = 1.1f;
    }
}