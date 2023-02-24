using UnityEngine;

namespace Prototype.AudioCore
{
    public class TestAudio : MonoBehaviour
    {
        private void Start()
        {
            PlayMusik();
        }
        
        public void PlaySound()
        {
            AudioController.PlaySound("dh-1");
        }
        
        private void PlayMusik()
        {
            AudioController.PlayMusic("Music");
        }
    }
}
