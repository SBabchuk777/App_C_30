using System.Collections;
using System.Collections.Generic;
using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    [RequireComponent(typeof(Toggle))]
    public class PlayOnToggleClick : MonoBehaviour
    {
        [SerializeField] private string _soundKey = "click";

        private void Awake()
        {
            Toggle toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener((active) =>
            {
                if (toggle.group != null && active == false)
                    return;

                AudioController.PlaySound(_soundKey);
            });
        }
    }
}