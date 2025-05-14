using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AdmobManager : SingletonMonoBehaviour<AdmobManager>
{
#if UNITY_ANDROID
    private const string _adUnitId_inter = "ca-app-pub-3940256099942544/1033173712";
    private const string _adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";
    private const string _adUnitId_aoa = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
    private const string _adUnitId_inter = "ca-app-pub-3940256099942544/1033173712";
    private const string _adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";
    private const string _adUnitId_aoa = "ca-app-pub-3940256099942544/9257395921";
#else
    private string _adUnitId_inter = "unused";
#endif

    public static ScreenOrientation orientation = ScreenOrientation.PortraitUpsideDown;

    private BannerView _bannerView;

    public void Setup()
    {
#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true);
#endif
        MobileAds.Initialize(initStatus =>
        {
            Debug.LogWarning("Load AOA ...");
            LoadAd();
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                AdmobManager.ins.RequestAndLoadInterstitialAd();
            });
        });
    }

    #region HELPER METHODS

    #endregion

    #region INTERSTITIAL ADS
    private InterstitialAd interstitialAd;
    private Action _onInter_Finish;
    private int interstitialRetryAttempt = 0;

    public bool ShowInterstitial(string placement = "", Action OnFinish = null)
    {
        try
        {
            if (IsInterReady())
            {
                AdsManager.ins.showingVideoAds = true;
                AdsManager.ins._timeWatchAdsLastest = 0;
                _onInter_Finish = OnFinish;

                interstitialAd.Show();
                return true;
            }
            else
            {
                PrintStatus("Interstitial ad is not ready yet.");
                FirebaseManager.ins.ads_inter_load_fail(AdsMediation.ADMOB.ToString());

                return false;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private void RequestAndLoadInterstitialAd()
    {
        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId_inter, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);

                    //Reload
                    interstitialRetryAttempt++;
                    double retryDelay = Math.Pow(2, Math.Min(4, interstitialRetryAttempt));
                    Invoke("RequestAndLoadInterstitialAd", (float)retryDelay);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());
                interstitialRetryAttempt = 0;
                interstitialAd = ad;

                RegisterEventHandlers(interstitialAd);
            });
    }

    private bool IsInterReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            // Extract the impression-level ad revenue data.
            double valueMicros = adValue.Value / 1000000f;

            // event firebase
            Firebase.Analytics.Parameter[] AdParameters = {
                    new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                    new Firebase.Analytics.Parameter("ad_source", "Admob Network"),
                    new Firebase.Analytics.Parameter("ad_unit_name", "Interstitial"),
                    new Firebase.Analytics.Parameter("ad_format", "Interstitial"),
                    new Firebase.Analytics.Parameter("currency", "USD"),
                    new Firebase.Analytics.Parameter("value", valueMicros),
                    };
            FirebaseManager.ins.ADS_RevenuePain(AdParameters);
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            FirebaseManager.ins.ads_inter_show(AdsMediation.ADMOB.ToString());
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");

            RequestAndLoadInterstitialAd();
            if (_onInter_Finish != null) _onInter_Finish.Invoke();
            FirebaseManager.ins.ads_inter_close(AdsMediation.ADMOB.ToString());
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            RequestAndLoadInterstitialAd();
            FirebaseManager.ins.ads_inter_display_fail(AdsMediation.ADMOB.ToString());
        };
    }

    #endregion

    #region BANNER ADS

    //Create + show banner (chỉ gọi duy nhất 1 lần)

    public void ShowBanner()
    {
        if (_bannerView == null) LoadBannerAd();
        _bannerView.Show();
    }

    public void HideBanner()
    {
        Debug.Log("Hide Banner");
        if (_bannerView != null) _bannerView.Hide();
    }

    private void LoadBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        //AdSize adaptiveSize =
        //        AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(_adUnitId_banner, AdSize.Banner, AdPosition.Bottom);

        var adRequest = new AdRequest();

        // Create an extra parameter that aligns the bottom of the expanded ad to the
        // bottom of the bannerView.
        adRequest.Extras.Add("collapsible", "bottom");

        ListenToAdEvents();

        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    /// 
    int bannerRetryAttempt = 0;
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);

            FirebaseManager.ins.LogEvent("af_Banner_Load_Fail_" + AdsMediation.ADMOB.ToString());
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            // Extract the impression-level ad revenue data.
            double valueMicros = adValue.Value / 1000000f;

            // event firebase
            Firebase.Analytics.Parameter[] AdParameters = {
                    new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                    new Firebase.Analytics.Parameter("ad_source", "Admob Network"),
                    new Firebase.Analytics.Parameter("ad_unit_name", "Open Ads"),
                    new Firebase.Analytics.Parameter("ad_format", "Open Ads"),
                    new Firebase.Analytics.Parameter("currency", "USD"),
                    new Firebase.Analytics.Parameter("value", valueMicros),
                    };
            FirebaseManager.ins.ADS_RevenuePain(AdParameters);
        };
    }

    #endregion

    #region AOA_ADS

    private static AppOpenAd _appOpenAd;

    public bool ShowAdIfAvailable()
    {

        //if (Application.isEditor) return true;

        if (!IsAdAvailable)
        {
            return false;
        }
        else
        {
            AdsManager.ins.showingVideoAds = true;
            AdsManager.ins.HideBanner();
            _appOpenAd.Show();
            FirebaseManager.ins.LogEvent("af_AOA_" + AdsMediation.ADMOB.ToString());
            return true;
        }
    }
    private bool IsAdAvailable
    {
        get
        {
            return _appOpenAd != null && _appOpenAd.CanShowAd();
        }
    }

    private void LoadAd()
    {
        if (_appOpenAd != null)
        {
            DestroyAd();
        }

        var adRequest = new AdRequest();

        AppOpenAd.Load(_adUnitId_aoa, adRequest, (AppOpenAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null || ad == null)
            {
                Debug.LogError("App open ad failed to load an ad with error : "
                                + error);

                //Call reload
                Invoke("LoadAd", 8f);
                return;
            }

            // The operation completed successfully.
            Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
            _appOpenAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(_appOpenAd);
        });
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // Extract the impression-level ad revenue data.
            double valueMicros = adValue.Value / 1000000f;

            // event firebase
            Firebase.Analytics.Parameter[] AdParameters = {
                    new Firebase.Analytics.Parameter("ad_platform", "Admob"),
                    new Firebase.Analytics.Parameter("ad_source", "Admob Network"),
                    new Firebase.Analytics.Parameter("ad_unit_name", "Open Ads"),
                    new Firebase.Analytics.Parameter("ad_format", "Open Ads"),
                    new Firebase.Analytics.Parameter("currency", "USD"),
                    new Firebase.Analytics.Parameter("value", valueMicros),
                    };
            FirebaseManager.ins.ADS_RevenuePain(AdParameters);

        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Displayed app open ad");
            AdsManager.ins.showingVideoAds = true;
            FirebaseManager.ins.LogEvent("af_AOA_Show_" + AdsMediation.ADMOB.ToString());
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Closed app open ad");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            ad = null;
            LoadAd();
            AdsManager.ins.showingVideoAds = false;
            FirebaseManager.ins.LogEvent("af_AOA_Close_" + AdsMediation.ADMOB.ToString());
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            //Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            ad = null;
            LoadAd();
            AdsManager.ins.showingVideoAds = false;
        };
    }

    public void DestroyAd()
    {
        if (_appOpenAd != null)
        {
            Debug.Log("Destroying app open ad.");
            _appOpenAd.Destroy();
            _appOpenAd = null;
            AdsManager.ins.ShowBanner();
        }
    }
    #endregion

    #region Utility

    ///<summary>
    /// Log the message and update the status text on the main thread.
    ///<summary>
    private void PrintStatus(string message)
    {
        Debug.Log(message);
    }

    #endregion
}
