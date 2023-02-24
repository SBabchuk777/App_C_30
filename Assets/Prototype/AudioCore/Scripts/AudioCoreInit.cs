using UnityEngine;
using Prototype.AudioCore;
using AudioSettings = Prototype.AudioCore.AudioSettings;

namespace Prototype
{
    public class AudioCoreInit : MonoBehaviour
    {
        private void Awake ()
        {
            DontDestroyOnLoad(gameObject);

            AudioSettings.UpdateSettings ( );
			
            AudioController.InitStreams(gameObject);
			
            LanguageAudio.Init();
            
            //DG.Tweening.DOTween.Init ( false, true, DG.Tweening.LogBehaviour.Verbose ).SetCapacity ( 200, 10 );
        }
    }
}
