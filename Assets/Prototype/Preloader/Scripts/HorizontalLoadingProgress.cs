using UnityEngine;
using Prototype.SceneLoaderCore.Helpers;
using UnityEngine.UI;

namespace Prototype.Preloader
{
    public class HorizontalLoadingProgress : LoadingProgress
    {
        [SerializeField] private Slider progress;

        
        protected override void StartLoading()
        {
            progress.value = 0.0f;
        }

        private void Update()
        {
            if (progress.value < 0.9f) progress.value += 0.8f * Time.deltaTime;
        }
    }
}