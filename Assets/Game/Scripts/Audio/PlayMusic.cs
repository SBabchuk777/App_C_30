using System.Collections;
using System.Collections.Generic;
using Prototype.AudioCore;
using UnityEngine;

namespace Audio
{
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField] private string _musicKey = "music";

        private void Awake()
        {
            if (!AudioController.IsSoundPlaying(_musicKey))
                AudioController.PlayMusic(_musicKey);
        }
    }
}