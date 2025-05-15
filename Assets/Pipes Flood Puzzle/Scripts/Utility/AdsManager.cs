using System;
using System.Collections;
using UnityEngine;

public class AdsManager : SingletonMonoBehaviour<AdsManager>
{
    #region awake
    [HideInInspector] public bool isIOS;
    public override void Awake()
    {
#if UNITY_IOS || UNITY_IPHONE
        isIOS = true;
#endif
        base.Awake();
    }
    #endregion

    private void OnEnable()
    {
        SetupAds();
    }

    public void SetupAds()
    {
        AdmobManager.ins.Setup();
        MaxManager.ins.Setup();
    }

    [HideInInspector] public float _timeWatchAdsLastest;
    [HideInInspector] public bool showingVideoAds = false;

    public void Update()
    {
        _timeWatchAdsLastest += Time.deltaTime;
    }

    #region INTERSTITIAL ADS
    public void ShowInterstitial(string placement = "", Action OnFinish = null)
    {
        if (AdsSDK.ins.isNoAds
            || AdsSDK.ins.isMkt
            || !AdsSDK.ins.canShowAds
            || (AdsManager.ins._timeWatchAdsLastest < AdsSDK.ins.timeAdsCapping))
        {
            if (OnFinish != null) OnFinish?.Invoke();
            return;
        }

        //Log Event
        FirebaseManager.ins.ads_inter_click();

        //Show ads ở net có độ ưu tiên cao trước
        //Nếu không có ads ở net ưu tiên cao hơn thì show ads ở net
        //có độ ưu tiên thấp hơn, cho đến tất cả các net
        bool check = false;
        for (var i = 0; i < AdsSDK.ins.adsPriority.Count; i++)
        {
            if (ShowAdsPriority(i, placement, OnFinish))
            {
                AdsManager.ins._timeWatchAdsLastest = 0;
                AdsManager.ins.showingVideoAds = true;
                check = true;
                if (AdsSDK.ins.isUseMediationTurn) ChangePriority(i);
                return;
            }
        }
        if (!check) if (OnFinish != null) OnFinish?.Invoke();
    }

    /// <summary>
    /// Thứ tự show ads của các net, bắt đầu từ 1
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
    public bool ShowAdsPriority(int priority, string placement = "", Action OnFinish = null)
    {
        var net = AdsSDK.ins.adsPriority.Find(x => x.priority == priority).adsMediation;
        switch (net)
        {
            case AdsMediation.MAX:
                return ShowAds_MAX(placement, OnFinish);
            case AdsMediation.ADMOB:
                return ShowAds_ADMOB();
        }
        return false;
    }

    /// <summary>
    /// Chuyển net vừa show thành công xuống độ ưu tiên cuối cùng
    /// </summary>
    public void ChangePriority(int priority)
    {
        try
        {
            for (var i = 0; i < AdsSDK.ins.adsPriority.Count; i++)
            {
                if (AdsSDK.ins.adsPriority[i].priority > priority && AdsSDK.ins.adsPriority[i].priority < AdsSDK.ins.adsPriority.Count)
                {
                    AdsSDK.ins.adsPriority[i].priority -= 1;
                }
            }
            AdsSDK.ins.adsPriority.Find(x => x.priority == priority).priority = AdsSDK.ins.adsPriority.Count - 1;
        }
        catch (Exception e)
        {
            Debug.Log("Ads SDK: Change error");
        }
    }

    public bool ShowAds_MAX(string placement = "", Action OnFinish = null)
    {
        return MaxManager.ins.ShowInterstitial(placement, () =>
        {
            if (OnFinish != null) OnFinish.Invoke();
        });
    }

    public bool ShowAds_ADMOB(string placement = "", Action OnFinish = null)
    {
        return AdmobManager.ins.ShowInterstitial(placement, () =>
        {
            if (OnFinish != null) OnFinish.Invoke();
        });
    }


    #endregion

    #region Reward
    public void ShowRewardedAd(string nameEvent = "", Action OnFinish = null, Action OnFail = null)
    {
        if (AdsSDK.ins.isMkt)
        {
            OnFinish?.Invoke();
            return;
        }

        //Nếu dùng Max
        MaxManager.ins.ShowRewardedAd(nameEvent, () =>
        {
            if (OnFinish != null) OnFinish.Invoke();
        }, () =>
        {
            if (OnFail != null) OnFail.Invoke();
        });

        //Nếu dùng Ironsource
        //....
    }
    #endregion

    #region Banner
    public void ShowBanner()
    {
        if (AdsSDK.ins.isNoAds) return;

        if(AdsSDK.ins.bannerMediationNetwork.Equals(AdsMediation.ADMOB.ToString()))
        {
            AdmobManager.ins.ShowBanner();
        }
        else if (AdsSDK.ins.bannerMediationNetwork.Equals(AdsMediation.MAX.ToString()))
        {
            MaxManager.ins.ShowBanner();
        }
    }

    public void HideBanner()
    {
        if (AdsSDK.ins.bannerMediationNetwork.Equals(AdsMediation.ADMOB.ToString()))
        {
            AdmobManager.ins.HideBanner();
        }

        else if (AdsSDK.ins.bannerMediationNetwork.Equals(AdsMediation.MAX.ToString()))
        {
            MaxManager.ins.HideBanner();
        }
    }
    #endregion

    #region AOA

    private float switchAppsTime;

    public void ShowAOA()
    {
        if (AdsSDK.ins.isNoAds) return;

        FirebaseManager.ins.LogEvent("af_AOA_attempt");
        AdmobManager.ins.ShowAdIfAvailable();
    }

    private void OnApplicationPause(bool paused)
    {
        //// Hiển thị AOA nếu back ra ngoài rồi vào lại > time capping
        if (paused)
        {
            //Lưu lại thời điểm User back ra ngoài
            switchAppsTime = GameHelper.CurrentTimeInSecond;
        }
        else
        {
            if (GameHelper.CurrentTimeInSecond - switchAppsTime < 2) return;
            if (AdsManager.ins.showingVideoAds)
            {
                AdsManager.ins.showingVideoAds = false;
                return;
            }
            else
            {
                ShowAOA();
            }
        }
    }
    #endregion

    #region Mrec
    public void ShowMrec(bool isHideBanner)
    {
        if (isHideBanner) HideBanner();
        MaxManager.ins.ShowMrec();
    }

    public void HideMrec(bool isShowBanner)
    {
        if (isShowBanner) ShowBanner();
        MaxManager.ins.HideMrec();
    }
    #endregion

}
