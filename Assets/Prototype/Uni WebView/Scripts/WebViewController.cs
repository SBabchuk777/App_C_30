using System;
using Prototype.MagicApps_Client;
using Prototype.Nointernet;
using Prototype.Preloader;
using Prototype.VerifyInternet;
using UnityEngine;

public class WebViewController : MonoBehaviour
{
    public static WebViewController Instance;
    
    [SerializeField, Header("Reference RectTransform")] private RectTransform _referenceRectTransform;
    
    [SerializeField, Header("Webview Background")] private GameObject _bgWebView;
    
    private UniWebView _webView;

    private string _url;

    private string UrlBinom
    {
        get
        {
            if(!PlayerPrefs.HasKey(Constants.IsFirstRunApp))
            {
                return PlayerPrefs.GetString(Constants.StartUrl, "");
            }
            else
            {
                return PlayerPrefs.GetString(Constants.StartNoRestore, "true") == "true"
                    ? PlayerPrefs.GetString(Constants.StartUrl, "")
                    : PlayerPrefs.GetString(Constants.LastUrl, "");
            }
        }

         set
        {
            PlayerPrefs.SetString(Constants.LastUrl, value);
            PlayerPrefs.Save();
        }
    }
    

    private bool isVisible;

    protected void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        
        SetAutoRotate();
    }

    private void Start()
    {
        Initialize();
    }
    
    private void SetAutoRotate()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    
    private void OnInitialize(bool? isConnection)
    {
        Debug.Log("### OnInitialize");
        
        Initialize(isConnection);
    }

    private void Initialize()
    {
        Debug.Log("### Initialize Webview");

        LoadUrl();
    }

    private void Initialize(bool? isConnection)
    {
        if (isConnection != true) return;
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.OnChangedInternetConnection.AddListener(OnInitialize);
            
        Initialize();
    }
    
    private void LoadUrl()
    {
        if (_webView == null) CreateWebView();
        
        _webView.Load(_url);

        _webView.OnPageErrorReceived += OnPageErrorReceived;

        _webView.OnPageFinished += OnPageFinished;
    }
    
    private void CreateWebView()
    {
        Debug.Log("### Create WebView");

        Debug.Log($"StartNoRestore :{PlayerPrefs.GetString(Constants.StartNoRestore, "true") }");
        Debug.Log($"StartUrl :{PlayerPrefs.GetString(Constants.StartUrl, "")}");
        Debug.Log($"LastUrl :{PlayerPrefs.GetString(Constants.LastUrl, "")}");
        Debug.Log($"_url :{UrlBinom}");
        _url = UrlBinom;
        
        var webViewGameObject = new GameObject("UniWebView");
        
        _webView = webViewGameObject.AddComponent<UniWebView>();
        
        if(_referenceRectTransform)
            _webView.ReferenceRectTransform = _referenceRectTransform;
        else
            _webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
    }
    
    private void OnPageErrorReceived(UniWebView view, int statusCode, string url)
    {
        Debug.Log("### Page Error Received Webview");

        _webView.OnPageErrorReceived -= OnPageErrorReceived;

        _webView.OnPageFinished -= OnPageFinished;
        
        HideWebView();
    }

    private void OnPageFinished(UniWebView view, int statusCode, string url)
    {
        Debug.Log($"### Page Finished Webview: url={url} / _webView.Url={_webView.Url}");
        
        if(url != "about:blank")
        {
            _url = _webView.Url;

            UrlBinom = url;
        }
            
        ShowWebView();
    }
    
    public void Back()
    {
        if(_webView) _webView.GoBack();
    }

    private void HideWebView()
    {
        Debug.Log("### Hide Webview");
        
        if(_webView == null) return;
        
        if (!isVisible) return;

        isVisible = false;
        
        //BackgroundVisible(false);

        _webView.Hide();
        
        NoInternet.Instance.Show();
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.CheckErrorReceived();
        
        if(ConnectivityManager.Instance)
            ConnectivityManager.Instance.OnChangedInternetConnection.AddListener(Initialize);
    }

    private void ShowWebView()
    {
        Debug.Log("### Show Webview: urlBinom="+UrlBinom);
        
        if(_webView == null) return;
        
        if (isVisible) return;

        isVisible = true;
        
        //BackgroundVisible(true);
        
        _webView.Show();
        
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
      
        if(LoadingProgress.Instance)
            LoadingProgress.Instance.Visible(false);
         
        PlayerPrefs.SetInt(Constants.IsFirstRunApp, 1);
        PlayerPrefs.Save();
    }

    private void BackgroundVisible(bool value)
    {
        if(_bgWebView) _bgWebView.SetActive(value);
    }
}
