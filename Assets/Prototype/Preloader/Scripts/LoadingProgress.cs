using UnityEngine;
using DG.Tweening;
using Prototype.SceneLoaderCore.Helpers;

namespace Prototype.Preloader
{
    public class LoadingProgress : MonoBehaviour
    {
        [SerializeField, Header("Progress")] private Transform loadingProgress;

        [SerializeField, Range(1.0f, 100.0f), Header("Angle Rotation")] private float rotationAngle = 100f;
        
        [SerializeField, Range(1.0f, 10.0f), Header("Speed Rotation")] private float rotationSpeed = 3f;

        private Tween _tween;

        private void OnEnable()
        {
            StartLoading();
        }
        
        private void OnDisable()
        {
            FinishActiveTween();
        }

        private void StartLoading()
        {
            _tween = loadingProgress.DORotate(Vector3.back * rotationAngle, rotationSpeed)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        public void StopLoading(bool webViewIsLoaded)
        {
            if(!webViewIsLoaded)
            {
                SceneLoader.Instance.SwitchToScene(SceneLoader.Instance.mainScene);
            }
            else
            {
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            
            FinishActiveTween();
        }
        
        private void FinishActiveTween()
        {
            if(_tween.IsPlaying()) _tween.Kill();
        }
    }
}