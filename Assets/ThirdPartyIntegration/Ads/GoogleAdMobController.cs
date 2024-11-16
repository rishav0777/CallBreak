using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class GoogleAdMobController : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    #region UNITY MONOBEHAVIOR METHODS

    public static GoogleAdMobController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void OnDestroy()
    {
        DestroyBannerAd();
        DestroyInterstitialAd();
    }

    #endregion

    #region HELPER METHODS
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            RequestBannerAd();
            RequestInterstitialAd();
        });
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #endregion

    #region BANNER ADS

    public void RequestBannerAd()
    {
        // These ad units are configured to always serve test ads.

#if DEVELOPMENT_BUILD && UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif DEVELOPMENT_BUILD && UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#elif UNITY_ANDROID
        string adUnitId = AdMobIDs.ANDROID_BANNER_ID;
#elif UNITY_IPHONE
        string adUnitId = AdMobIDs.IOS_BANNER_ID;
#endif
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Add Event Handlers
        bannerView.OnAdLoaded += OnBannerLoaded;

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    private void OnBannerLoaded(System.Object sender, EventArgs args)
    {
        bannerView.Show();
    }

    #endregion

    #region INTERSTITIAL ADS

    private void RequestInterstitialAd()
    {
        // These ad units are configured to always serve test ads.

#if DEVELOPMENT_BUILD && UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif DEVELOPMENT_BUILD && UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#elif UNITY_ANDROID
        string adUnitId = AdMobIDs.ANDROID_INTERSTITIAL_ID;
#elif UNITY_IPHONE
        string adUnitId = AdMobIDs.IOS_INTERSTITIAL_ID;
#endif

        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        interstitialAd = new InterstitialAd(adUnitId);

        //interstitialAd.OnAdLoaded += OnInterstitialLoaded;
        interstitialAd.OnAdClosed += OnInterstitialClosed;

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());
    }

    private void OnInterstitialClosed(System.Object sender, EventArgs args)
    {
        RequestInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null)
        {
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.Show();
            }
            else
            {
                RequestInterstitialAd();
            }
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }
    #endregion
}