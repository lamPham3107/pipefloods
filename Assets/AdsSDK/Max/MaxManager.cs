using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxManager : SingletonMonoBehaviour<MaxManager>
{
#if UNITY_ANDROID
    private const string InterstitialAdUnitId = "f8150e3857b3db60";
    private const string RewardedAdUnitId = "a3b12b7fcc2aba1d";
    private const string BannerAdUnitId = "562cf8b43642ea67";
    private const string MrecAdUnitId = "c3621bcc8536bde7";
#elif UNITY_IPHONE
    private const string InterstitialAdUnitId = "";
    private const string RewardedAdUnitId     = "";
    private const string BannerAdUnitId       = "";
    private const string MrecAdUnitId         = "";
#else
    private string _adUnitId_inter = "unused";
#endif

    [Header("Action Event")]
    Action OnRewardAds_Complete, OnRewardAds_Fail;
    Action OnInter_Finish;

    [Header("Status")]
    private bool recieveReward = false;


    #region counting load time
    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;
    #endregion

    #region SETUP

    public void Setup()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debug.Log("MAX SDK Initialized");
            InitializeInterstitialAds();
            InitializeRewardedAds();
            //InitializeBannerAds();
            //InitializeMRecAds();
            //MaxSdk.ShowMediationDebugger();
        };
        MaxSdk.InitializeSdk();
    }
    #endregion

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds()
    {
        // Attach callbacks

        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        LoadInterstitial();
    }

    public void LoadInterstitial()
    {
        if (AdsSDK.ins.isNoAds) return;
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (IsLoadedInterstitial()) return;

        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }

    public bool ShowInterstitial(string placement = "", Action OnFinish = null)
    {
        try
        {
            OnInter_Finish = OnFinish;

            if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
            {
                MaxSdk.ShowInterstitial(InterstitialAdUnitId);
                return true;
            }
            else
            {
                FirebaseManager.ins.ads_inter_load_fail(AdsMediation.MAX.ToString());
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi Inter: " + e);
            return false;
        }
    }

    public bool IsLoadedInterstitial()
    {
        return MaxSdk.IsInterstitialReady(InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial loaded");
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.ToString());
    }

    private void OnInterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        //DebugCustom.Log("Interstitial failed to display with error code: " + errorCode);
        LoadInterstitial();
        if (OnInter_Finish != null) OnInter_Finish?.Invoke();
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.ToString());
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        AdsManager.ins._timeWatchAdsLastest = 0;
        LoadInterstitial();
        if (OnInter_Finish != null) OnInter_Finish?.Invoke();
        Debug.Log("Interstitial dismissed");

        FirebaseManager.ins.ads_inter_close(AdsMediation.MAX.ToString());
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial Displayed");

        //Log Event
        FirebaseManager.ins.ads_inter_show(AdsMediation.MAX.ToString());
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        FirebaseManager.ins.ADS_RevenuePain(impressionParameters);

    }

    #endregion

    #region Rewarded Ad Methods
    private string _rewardPlacement;
    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        LoadRewardedAd();
    }

    public bool isRewardedAvailable()
    {
        return MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
    }

    public void LoadRewardedAd()
    {
        //không có mạng thì thôi
        if (Application.internetReachability == NetworkReachability.NotReachable) return;

        //Có sẵn r thì ko cần load
        //Phòng trường hợp load liên tục
        if (isRewardedAvailable()) return;

        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    public bool ShowRewardedAd(string placement = "", Action OnFinish = null, Action OnFail = null)
    {
        Debug.Log("Call reward");
        this._rewardPlacement = placement;
        try
        {
            //Editor thì thôi, cho nó luôn
            if (Application.isEditor)
            {
                if (OnFinish != null) OnFinish?.Invoke();
                return true;
            }

            OnRewardAds_Complete = OnFinish;
            OnRewardAds_Fail = OnFail;

            recieveReward = false;

            //Event Log
            FirebaseManager.ins.ads_reward_click(_rewardPlacement);

            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
            {
                AdsManager.ins.showingVideoAds = true;
                Dictionary<string, string> value = new Dictionary<string, string>();
                MaxSdk.ShowRewardedAd(RewardedAdUnitId);
                return true;
            }
            else
            {
                Debug.LogWarning("Lỗi chưa load đc Video Ads");
                LoadRewardedAd();
                if (OnRewardAds_Fail != null) OnRewardAds_Fail?.Invoke();
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi VideoAds: " + e);
            if (OnRewardAds_Fail != null) OnRewardAds_Fail?.Invoke();
            return false;
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");
        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, rewardedRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.ToString());

        //Event Log
        FirebaseManager.ins.ads_reward_fail(_rewardPlacement, "Load Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        LoadRewardedAd();
        OnRewardAds_Fail?.Invoke();
        OnRewardAds_Fail = null;
        AdsManager.ins.showingVideoAds = false;
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.ToString());

        //Event Log
        FirebaseManager.ins.ads_reward_fail(_rewardPlacement, "Display Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        recieveReward = false;
        Debug.Log("Rewarded ad displayed");

        //Event Log
        FirebaseManager.ins.ads_reward_show(_rewardPlacement);
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        if (recieveReward)
        {
            recieveReward = false;
            AdsManager.ins._timeWatchAdsLastest = 0;
            AdsManager.ins.showingVideoAds = false;
            if (OnRewardAds_Complete != null) OnRewardAds_Complete?.Invoke();
            OnRewardAds_Complete = null;
        }
        else
        {
            AdsManager.ins._timeWatchAdsLastest = 0;
            AdsManager.ins.showingVideoAds = false;
            if (OnRewardAds_Fail != null) OnRewardAds_Fail?.Invoke();
            OnRewardAds_Fail = null;
        }
        Debug.Log("Rewarded ad dismissed");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        recieveReward = true;
        Debug.Log("Rewarded ad received reward");

        //Event Log
        FirebaseManager.ins.ads_reward_complete(_rewardPlacement);
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.

        // Ad revenue
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        FirebaseManager.ins.ADS_RevenuePain(impressionParameters);

    }

    #endregion

    #region Banner Ad Methods
    private void InitializeBannerAds()
    {
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        //MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        //Load Banner
        LoadBanner();
    }

    void LoadBanner()
    {
        MaxSdk.LoadBanner(BannerAdUnitId);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        FirebaseManager.ins.ADS_RevenuePain(impressionParameters);

    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }



    public void ShowBanner()
    {
        if (Application.isEditor) return;
        if (AdsSDK.ins.isNoAds) return;
        //LoadBanner();
        MaxSdk.ShowBanner(BannerAdUnitId);
    }

    public void HideBanner()
    {
        try
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }
        catch (Exception e)
        {

        }
    }

    public void ShowBannerInBottom()
    {
        if (Application.isEditor) return;
        if (AdsSDK.ins.isNoAds) return;
        //LoadBanner();
        try
        {
            //LoadBanner();
            MaxSdk.ShowBanner(BannerAdUnitId);
            MaxSdk.UpdateBannerPosition(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        }
        catch (Exception e)
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }
    }

    public void ShowBannerInTop()
    {
        if (Application.isEditor) return;
        if (AdsSDK.ins.isNoAds) return;
        try
        {
            //LoadBanner();
            MaxSdk.ShowBanner(BannerAdUnitId);
            MaxSdk.UpdateBannerPosition(BannerAdUnitId, MaxSdkBase.BannerPosition.TopCenter);
        }
        catch (Exception e)
        {
            MaxSdk.HideBanner(BannerAdUnitId);
        }

    }
    #endregion

    #region Mrec Ad Methods
    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(MrecAdUnitId, MaxSdkBase.AdViewPosition.BottomCenter);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;

        MaxSdk.LoadMRec(MrecAdUnitId);
    }

    public void ShowMrec()
    {

        try
        {
            if (Application.isEditor
                || AdsSDK.ins.isNoAds)
            {
                return;
            }
            Debug.Log("Show Mrec");
            MaxSdk.ShowMRec(MrecAdUnitId);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi Inter: " + e);
        }
    }

    public void HideMrec()
    {
        try
        {
            if (Application.isEditor
                || AdsSDK.ins.isNoAds)
            {
                return;
            }

            MaxSdk.HideMRec(MrecAdUnitId);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lỗi Inter: " + e);
        }
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
            new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
            new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
            new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        FirebaseManager.ins.ADS_RevenuePain(impressionParameters);
    }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion
}

[System.Serializable]
public class ImpressionDataCustom
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;
}