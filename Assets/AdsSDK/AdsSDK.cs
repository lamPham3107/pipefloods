using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// **** Appsflyer config:
/// Thêm: AppsFlyer.setHost("", "register.appsflyersdk.com");
/// vào đầu Start() của AppsflyerObjectScript.cs
/// </summary>
public class AdsSDK : SingletonMonoBehaviour<AdsSDK>
{
    #region System Register
    #endregion

    #region Remote Config Variable
    [Header("[REMOTE CONFIG SETTING]")]
    /// <summary>
    /// Default Variable
    /// </summary>
    public float timeAdsCapping = 25;
    public int levelStartShowAds;
    public string aoaMediationNetwork = "ADMOB";
    public string bannerMediationNetwork = "ADMOB";

    /// <summary>
    /// Thứ tự ưu tiên của network mediation (remote bởi firebase -> cần tạo các biến tương ứng và set thứ tự)
    /// ex: MAX - 1, ADMOB - 2, YANDEX - 3
    /// Nếu muốn đổi thứ tự thì phải đổi tương ứng số (bắt đầu từ 1)
    /// </summary>
    public List<AdsMediationPriority> adsPriority = new List<AdsMediationPriority>();
    public bool isUseMediationTurn = false;

    /// <summary>
    /// Other Variable
    /// </summary>

    #endregion

    #region MKT Config
    [Header("[MKT SETTING]")]
    public bool isMkt;
    public bool canShowAds;
    #endregion

    #region Player System Data
    [Header("[PLAYER DATA - NOT EDIT]")]
    public bool isNoAds;
    public int timeInstall;
    public int timeLastOpen;
    public int daysPlayed;
    public int dayLastOpen;
    #endregion

    #region Start Schedule
    [HideInInspector] public bool _isAdsSetupDone;
    IEnumerator Start()
    {
        _isAdsSetupDone = false;

        #region  LOAD-SAVE DATA SYSTEM
        //LOAD
        isNoAds = PlayerPrefs.GetInt("NoAds", 0) == 1;
        timeInstall = PlayerPrefs.GetInt("timeInstall", GameHelper.CurrentTimeInSecond);
        timeLastOpen = PlayerPrefs.GetInt("timeLastOpen", GameHelper.CurrentTimeInSecond);
        dayLastOpen = PlayerPrefs.GetInt("dayLastOpen", GameHelper.GetDayNow);
        daysPlayed = PlayerPrefs.GetInt("daysPlayed", 0);
        if (GameHelper.GetDayNow - dayLastOpen > 0)
        {
            daysPlayed += 1;
        }

        //SAVE
        PlayerPrefs.SetInt("NoAds", isNoAds ? 1 : 0);
        PlayerPrefs.SetInt("timeInstall", timeInstall);
        PlayerPrefs.SetInt("timeLastOpen", GameHelper.CurrentTimeInSecond);
        PlayerPrefs.SetInt("dayLastOpen", GameHelper.GetDayNow);
        PlayerPrefs.SetInt("daysPlayed", daysPlayed);
        #endregion

        //Đợi khởi tạo các đối tượng
        yield return new WaitUntil(() =>
            AdsManager.ins != null
            && MaxManager.ins != null
            && AdmobManager.ins != null
            && FirebaseManager.ins != null);

        #region Setup Ads
        AdsManager.ins.SetupAds();
        #endregion

        #region Setup Firebase
        {
            //Wait Firebase RemoteConfig done
            var check = false;
            Timer.Schedule(this, 3f, () =>
            {
                check = true;
            });
            yield return new WaitUntil(() => FirebaseManager.ins.is_remote_config_done || check);
        }
        #endregion

        PlayerPrefs.SetInt("FirstGame", 1);

        AdsManager.ins.ShowAOA();
        //AdsManager.ins.ShowBanner();    

        _isAdsSetupDone = true;       
        FirebaseManager.ins.LogEvent("open_app");


    }

    #endregion

}

[System.Serializable]
public class AdsMediationPriority
{
    public AdsMediation adsMediation;
    public int priority;

    public AdsMediationPriority (AdsMediation adsMediation, int priority)
    {
        this.adsMediation = adsMediation;
        this.priority = priority;
    }
}

[System.Serializable]
public enum AdsMediation
{
    MAX,
    ADMOB,
    YANDEX,
    ADMOB_MEDIATION,
    YANDEX_MEDIATION
}