using UnityEngine;
using UnityEngine.UI;
using static Prototype.AudioCore.AudioController;

namespace Prototype.AudioCore
{
    public class SwitchEnableStrems : MonoBehaviour
    {
        [Header("Group")]
        public StreamGroup group;

        private Image _bttn;

        private void Awake()
        {
            _bttn = GetComponent<Image>();
        }

        private void Start()
        {
            UpdateBttn();
        }
        
        public void SetEnable()
        {
            if (group == StreamGroup.Music)
            {
                AudioSettings.EnableMusic(!AudioSettings.IsMusicEnabled());
            }
            else
            {
                AudioSettings.EnableSounds(!AudioSettings.IsSoundsEnabled());
            }

            UpdateBttn();
        }

        private void UpdateBttn()
        {
            var clr = group == StreamGroup.Music ? (AudioSettings.IsMusicEnabled() ? Color.green : Color.red) : (AudioSettings.IsSoundsEnabled() ? Color.green : Color.red);

            _bttn.color = clr;
        }
    }
}
