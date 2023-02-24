using System.Collections;
using System.Collections.Generic;
using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    [RequireComponent(typeof(Button))]
    public class PlayOnClick : MonoBehaviour
    {
        [SerializeField] private string _soundKey = "click";

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
                AudioController.PlaySound(_soundKey));
        }
    }
}