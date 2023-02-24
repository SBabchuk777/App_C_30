using System;
using Prototype.Logger;
using UnityEngine;
using UnityEngine.Events;

public class WebViewController : MonoBehaviour
{
    [SerializeField, Header("Remote Config")] private FirebaseRemoteConfig firebaseRemoteConfig;

    [SerializeField, Header("Reference RectTransform")] private RectTransform _referenceRectTransform;
    
    [SerializeField, Header("Webview Background")] private GameObject _bgWebView;
    
    [Space(10)]
    
    [SerializeField] private UnityEvent WebViewLoadingError;

    [SerializeField] private UnityEvent WebViewLoadingCompleted;
    
    [SerializeField] private UnityEvent DefaultBinomLoadingCompleted;
    
    private UniWebView _webView;

    private string urlBinom;

    private bool isVisible;
    
    public void Initialize()
    {
        DLogger.Log("### Initialize");
        
        if (_webView)
        {
            DLogger.Log("### Initialize -> LoadUrl");
            LoadUrl();
        }
        else
        {
            DLogger.Log("### Initialize -> InitRemoveConfig");
#if UNITY_EDITOR
            DefaultBinomLoadingCompleted?.Invoke();
#else
                if (firebaseRemoteConfig != null)
                    firebaseRemoteConfig.InitRemoveConfig(CheckUrl);
                else
                    DefaultBinomLoadingCompleted?.Invoke();
#endif
        }
    }
    
    public void Back()
    {
        if(_webView) _webView.GoBack();
    }
    
    private void CheckUrl()
    {
        DLogger.Log("@@@ CheckUrl");
        
        urlBinom = firebaseRemoteConfig.GetURL();
        
        DLogger.Log($"@@@ urlBinom: {urlBinom}");
        
        if (String.IsNullOrEmpty(urlBinom))
        {
            DefaultBinomLoadingCompleted?.Invoke();

            return;
        }

        DLogger.Log("@@@ CheckUrl -> LoadUrl");
        LoadUrl();
    }

    private void CreateWebView()
    {
        var webViewGameObject = new GameObject("UniWebView");
        
        _webView = webViewGameObject.AddComponent<UniWebView>();
        
        if(_referenceRectTransform)
            _webView.ReferenceRectTransform = _referenceRectTransform;
        else
            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
    }

    private void LoadUrl()
    {
        DLogger.Log($"@@@ LoadUrl:: _webView ?= null:{_webView == null}");
        if (_webView == null) CreateWebView();

        _webView.Load(urlBinom);

        _webView.OnPageErrorReceived += OnPageErrorReceived;

        _webView.OnPageFinished += OnPageFinished;
    }
    
    private void OnPageErrorReceived(UniWebView view, int statusCode, string url)
    {
        DLogger.Log(" ----- Hide WebView in WebView");
        HideWebView();
        
        _webView.OnPageErrorReceived -= OnPageErrorReceived;

        _webView.OnPageFinished -= OnPageFinished;
    }

    private void OnPageFinished(UniWebView view, int statusCode, string url)
    {
        DLogger.Log($"@@@ OnPageFinished: {url}");
        
        if(url != "about:blank")
            urlBinom = _webView.Url;
            
        if(firebaseRemoteConfig.isUrlDefaultBinom(url))
            RemoveWebView();
        else
            ShowWebView();
    }

    private void RemoveWebView()
    {
        if(_bgWebView) _bgWebView.SetActive(false);
        
        DefaultBinomLoadingCompleted?.Invoke();

        Destroy(_webView);
        
        _webView = null;
    }
    
    public void HideWebView()
    {
        if(_webView == null) return;
        
        if (!isVisible) return;

        isVisible = false;
        
        DLogger.Log(" --- Hided");

        if(_bgWebView) _bgWebView.SetActive(false);

        _webView.Hide();
        
        WebViewLoadingError?.Invoke();
    }

    private void ShowWebView()
    {
        if(_webView == null) return;
        
        if (isVisible) return;

        isVisible = true;
        
        if(_bgWebView) _bgWebView.SetActive(true);

        _webView.Show();
        
        WebViewLoadingCompleted?.Invoke();
    }
}
