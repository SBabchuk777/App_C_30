using Prototype.AudioCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using AudioSettings = Prototype.AudioCore.AudioSettings;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private StreamGroup _stream = StreamGroup.Music;

        private void Awake()
        {
            Slider slider = GetComponent<Slider>();

            if (_stream == StreamGroup.Music)
                slider.value = AudioSettings.GetMusicVol();

            if (_stream == StreamGroup.FX)
                slider.value = AudioSettings.GetSoundsVol();

            slider.onValueChanged.AddListener((volume) =>
            {

                if (_stream == StreamGroup.Music)
                    AudioSettings.SetMusicVol(slider.value);

                if (_stream == StreamGroup.FX)
                    AudioSettings.SetSoundsVol(slider.value);
            });
        }

        public void OnPointerUp(PointerEventData eventData) =>
            AudioController.PlaySound("slider");
    }
}