using System.Collections;
using System.Collections.Generic;
using Prototype.SceneLoaderCore.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName = "Scene";

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
                SceneLoader.Instance.SwitchToScene(_sceneName));
        }
    }
}