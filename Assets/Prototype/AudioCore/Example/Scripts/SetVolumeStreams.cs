using UnityEngine;
using UnityEngine.UI;

namespace Prototype.AudioCore
{
    public class SetVolumeStreams : MonoBehaviour
    {
        [Header("Group")] 
        public StreamGroup group;

        private Slider _slider;
        
        public void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        private void Start()
        {
            UpdateSlider();
        }
        
        public void SetVolume()
        {
            if (group == StreamGroup.Music)
            {
                AudioSettings.SetMusicVol(_slider.value);
            }
            else
            {
                AudioSettings.SetSoundsVol(_slider.value);
            }
        }
        
        private void UpdateSlider()
        {
            _slider.value = group == StreamGroup.Music 
                            ? AudioSettings.GetMusicVol() 
                            : AudioSettings.GetSoundsVol();
        }
    }
}
