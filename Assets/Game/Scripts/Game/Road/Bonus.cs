using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Road
{
    public class Bonus : MonoBehaviour
    {
        [SerializeField] private int _points = 5;

        public int BonusPoints => _points;
    }
}