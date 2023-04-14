using UnityEngine;

public class WebviewShowPage : MonoBehaviour
{
    [SerializeField, Header("Embedded Toolbar")] private bool _embeddedToolbar;
    
    [SerializeField, Header("Reference RectTransform")] private RectTransform _referenceRectTransform;
    
    [SerializeField, Header("Webview Background")] private GameObject _bgWebView;
    
    [SerializeField, Header("No Internet")] private GameObject _noInternet;
     
    private UniWebView _webView;
    
    public void LoadUrl(string urlBinom = "http://google.com")
    {
        if (_webView == null) CreateWebView();

        _webView.Load(urlBinom);

        _webView.OnPageErrorReceived += OnPageErrorReceived;

        _webView.OnPageFinished += OnPageFinished;
        
        _webView.OnShouldClose += OnPageClosed;
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
    
    private void OnPageErrorReceived(UniWebView view, int statusCode, string url)
    {
        RemoveWebView();
        
        _noInternet.SetActive(true);
        
        _webView.OnPageErrorReceived -= OnPageErrorReceived;

        _webView.OnPageFinished -= OnPageFinished;
        
        _webView.OnShouldClose -= OnPageClosed;
    }
    
    private bool OnPageClosed(UniWebView webview)
    {
        if(_bgWebView) _bgWebView.SetActive(false);

        return _bgWebView;
    }
    
    private void OnPageFinished(UniWebView view, int statusCode, string url)
    {
        ShowWebView();
    }
  
    private void RemoveWebView()
    {
        if(_bgWebView) _bgWebView.SetActive(false);
        
        Destroy(_webView);
        
        _webView = null;
    }

    public void HideWebView()
    {
        RemoveWebView();
    }
    
    private void ShowWebView()
    {
        if(_webView == null) return;
        
        if(_bgWebView) _bgWebView.SetActive(true);
        
        _webView.Show();
        
        if(_embeddedToolbar)
            _webView.EmbeddedToolbar.Show();
    }
}
