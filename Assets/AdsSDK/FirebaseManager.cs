using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : SingletonMonoBehaviour<FirebaseManager>
{
    //Check khởi tạo firebase
    private bool fireBaseReady = false;//Firebase đã Init thành công
    private bool firebaseIniting = true;//Firebase đang Init

    [HideInInspector] public bool is_remote_config_done = false;//Quá trình RemoteConfig đã xong
    [HideInInspector] public bool is_remote_config_success = false;//RemoteConfig thành công

    #region FIREBASE SETUP
    public override void Awake()
    {
        base.Awake();
        firebaseIniting = true;
    }

    private void Start()
    {
        CheckFireBase();
    }

    private void CheckFireBase()
    {
        try
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                firebaseIniting = false;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    fireBaseReady = true;

                    //Remote Config
                    fetch((bool is_fetch_result) => { });

                    Firebase.Analytics.FirebaseAnalytics.SetUserProperty("day", GameHelper.GetUserLoginDay());
                }
                else
                {
                    Debug.LogError(System.String.Format("Dependencies Firebase Error: {0}", dependencyStatus));
                }
            });
        }
        catch (System.Exception ex)
        {
            firebaseIniting = false;
            Debug.LogError("Firebase Init Error:" + ex.ToString());
        }
    }
    #endregion

    #region USER_PROPERTIES
    //Call When Setup Done
    public void OnSetUserProperty()
    {
        StartCoroutine(ie_OnSetUserProperty());
    }
    //Hàm này gọi 2 lần:
    //1. Khi mở game
    //2. Khi win 1 level (đối với game hyper) hoặc win 1 level ở main game content (với các game mid/puzzle)
    IEnumerator ie_OnSetUserProperty()
    {
        if (Application.isEditor) yield break;

        yield return new WaitUntil(() => fireBaseReady);
        try
        {
            //===========================================================
            //retentType
            //[timeInstall]: Thời gian cài app lần đầu
            var timeInstall = AdsSDK.ins.timeInstall;

            //===========================================================
            //[timeLastOpen] : Thời gian mở app lần cuối (hiện tại)
            var time = AdsSDK.ins.timeLastOpen;

        }
        catch (Exception e)
        {
            Debug.LogError("Firebase: (Userproperties) Error: " + e);
        }
    }

    private void SetProperty(string key, object value)
    {
        try
        {
            FirebaseAnalytics.SetUserProperty(key.ToString(), value.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Error UserProperty Firebase: " + key + " _ " + e);
        }
    }
    #endregion

    #region Events

    #region Base
    public void LogEvent(string name)
    {
        if(fireBaseReady) FirebaseAnalytics.LogEvent(name);
    }

    //OPTION: không yêu cầu
    //Mỗi 30s chơi game (tổng thời gian chơi) tính là 1 checkpoint.
    //[name_game_cur]: tên game hiện tại đang chơi
    //[name_game_max]: tên game cao nhất đã chơi (nếu là các game có nhiều trò như octopus, nếu không thì 2 thông số sẽ là level đang chơi)
    public void check_point_time(int check_point, string name_game_cur, string name_game_max)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            if (check_point < 10)
            {
                FirebaseAnalytics.LogEvent("check_point_time", new Parameter[]
                {
                    new Parameter("checkpoint_0", check_point),
                    new Parameter("name_game_cur", name_game_cur),
                    new Parameter("name_game_max", name_game_max)
                });
            }
            else
            {
                FirebaseAnalytics.LogEvent("check_point_time", new Parameter[]
                {
                    new Parameter("checkpoint_", check_point),
                    new Parameter("name_game_cur", name_game_cur),
                    new Parameter("name_game_max", name_game_max)
                });
            }
        }
    }

    //Call mỗi khi start level
    //[level]: mốc level
    //[stage]: tên stage của level hiện tại đang chơi (tùy mỗi game mà có hay không)
    public void check_point_level(int level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            if (level < 10)
            {
                FirebaseAnalytics.LogEvent("start_level", new Parameter[]
                {
                    new Parameter("level_0", level)
                });
            }
            else
            {
                FirebaseAnalytics.LogEvent("start_level", new Parameter[]
                {
                    new Parameter("level_", level)
                });
            }
        }
    }

    //Call mỗi khi end level
    //[level]: mốc level (level 0 -> mốc 1, level 1 -> mốc 2 ...)
    //[stage]: tên stage của level hiện tại đang chơi (tùy mỗi game mà có hay không)
    public void check_point_end(int level, bool isWin)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            if (level < 10)
            {
                FirebaseAnalytics.LogEvent("end_level", new Parameter[]
                {
                    new Parameter("level_0", level),
                    new Parameter("is_win", isWin ? "win" : "lose")
                });
            }
            else
            {
                FirebaseAnalytics.LogEvent("end_level", new Parameter[]
                {
                    new Parameter("level_", level),
                    new Parameter("is_win", isWin ? "win" : "lose")
                });
            }
        }
    }

    //[nameGame]: 
    //[timeplayed]: thời gian chơi
    public void level_complete(string level, int timeplayed)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("level_complete", new Parameter[]
            {
                new Parameter("level", level),
                new Parameter("timeplayed", timeplayed.ToString())
            });
        }
    }

    //[nameGame]:
    //[failcount]: số lần fail của level 
    public void level_fail(string level, int failcount)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("level_fail", new Parameter[]
            {
                new Parameter("level", level),
                new Parameter("failcount", failcount.ToString()),
            });
        }
    }

    //Loại tiền tệ kiếm được
    //[virtual_currency_name]: tên loại tiền tệ (gold, gem ...)
    //[amount]: số lượng
    //[source]: nguồn kiếm được (collect in game, view reward, buy with $ ...)
    public void earn_virtual_currency(string virtual_currency_name, int amount, string source)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("earn_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", amount),
                new Parameter("source", source)
               });
        }
    }

    //Loại tiền tệ tiêu thụ
    //[virtual_currency_name]: tên loại tiền tệ (gold, gem ...)
    //[amount]: số lượng
    //[source]: nguồn tiêu thụ (mua skin, hồi sinh, ...)
    public void spend_virtual_currency(string virtual_currency_name, int amount, string item_name)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("spend_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", amount),
                new Parameter("item_name", item_name)
               });
        }
    }
    #endregion

    #region Order Of Publisher

    #region Reward ADS
    //Khi click vào button reward
    //(điều kiện là đã load được ads -> nên block - làm mờ nút ads reward nếu không load đc ads hoặc ko có mạng)
    //[placement]: vị trí hiển thị (endgame, shop ...)
    public void ads_reward_click(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("reward_click", new Parameter[]
            {
                new Parameter("placement", placement)
            });
        }
    }

    //Khi reward được show lên (hiển thị thành công)
    public void ads_reward_show(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_reward", new Parameter[]
            {
                new Parameter("placement", placement)
            });
        }
    }

    //Khi reward bị lỗi khi hiển thị 
    //[placement]: vị trí hiển thị (endgame, shop ...)
    //[errormsg]: tên lỗi: Error Message: Unknown,Offline,NoFill,InternalError,InvalidRequest,UnableToPrecached
    //[level]: level hiển thị
    public void ads_reward_fail(string placement, string errormsg)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("reward_fail", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("errormsg", errormsg)
            });
        }
    }

    //Khi reward có thể nhận thưởng
    //Call khi close reward ads hoặc event có thể nhận thưởng
    public void ads_reward_complete(string placement)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("reward_complete", new Parameter[]
            {
                new Parameter("placement", placement),
            });
        }
    }
    #endregion

    #region Inter ADS
    //Khi gọi inter ads
    public void ads_inter_click()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inter_attempt", new Parameter[] { });
        }
    }

    //Khi ads inter hiển thị (trong sự kiện ads trả về -> chắc chắn sẽ hiện)
    public void ads_inter_show(string net)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inters_" + net, new Parameter[] { });
        }
    }

    //Khi ads inter hiển thị thành công và user đóng quảng cáo chơi tiếp (trong sự kiện ads trả về -> chắc chắn sẽ hiện)
    public void ads_inter_close(string net)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inters_close" + net, new Parameter[] { });
        }
    }

    //Khi ads inter hiển thị lỗi
    //Error Message: FailToLoad, Unavailable
    public void ads_inter_display_fail(string net)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inter_display_fail_" + net, new Parameter[] { });
        }
    }


    public void ads_inter_load_fail(string net)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inter_load_fail_" + net, new Parameter[] { });
        }
    }

    public void ads_inter_load_fail_lastest()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("af_inter_load_fail_lastest", new Parameter[] { });
        }
    }
    #endregion

    #region ADS RevenuePain
    /// <summary>
    /// Send theo event của Max Manager (Log doanh thu từ mỗi quảng cáo)
    /// </summary>
    /// <param name="data"></param>
    public void ADS_RevenuePain(Parameter[] AdParameters)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
        }
    }
    #endregion

    #region Other

    public void E_LevelStart(int level)
    {
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_start", new Parameter[]
                {
                    new Parameter("level", level.ToString()),
                    new Parameter("day", GameHelper.GetUserLoginDay()),
                });
            }
        }
        catch (Exception e)
        {

        }
    }

    public void E_LevelComplete(int level, int star, int retry)
    {
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_complete", new Parameter[]
                {
                    new Parameter("level", level.ToString())
                });

                PlayerPrefs.SetInt("first_win_level_" + level, 1);
            }
        }
        catch (Exception e)
        {

        }
    }

    public void E_LevelFail(int level, string reason)
    {
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_fail", new Parameter[]
                {
                    new Parameter("level", level.ToString()),
                    new Parameter("reason", reason.ToString())
                });
            }
        }
        catch (Exception e)
        {

        }
    }

    public void E_RewardComplete(string placement, int level)
    {
        try
        {
            FirebaseAnalytics.LogEvent("ads_reward_complete", new Parameter[]
            {
                new Parameter("placement", placement),
                new Parameter("level", level.ToString()),
                new Parameter("day", GameHelper.GetUserLoginDay()),
            });
        }
        catch (Exception e)
        {

        }
    }

    public void E_InterShow(int level)
    {
        try
        {
            FirebaseAnalytics.LogEvent("ad_inter_show", new Parameter[]
            {
                new Parameter("level", level.ToString()),
                new Parameter("day", GameHelper.GetUserLoginDay())
            });
        }
        catch (Exception e)
        {

        }
    }

    #endregion
    #endregion

    #endregion

    #region Remote Config
    /// <summary>
    /// Setup Remote config
    /// </summary>
    /// <param name="completionHandler"></param>
    public void fetch(Action<bool> completionHandler)
    {
        return;
        try
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();


            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

            var settings = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings);

            var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

            fetchTask.ContinueWithOnMainThread(task =>
            {
                is_remote_config_done = true;
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogWarning("fetchTask Firebase Fail");
                    is_remote_config_success = false;
                    completionHandler(false);
                }
                else
                {
                    Debug.LogWarning("fetchTask Firebase Commplete");
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    RefrectProperties();

                    completionHandler(true);
                }
            });
        }
        catch (Exception ex)
        {
            fetch((bool is_fetch_result) => { });
            //is_remote_config_done = true;
            Debug.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Dữ liệu remote config
    /// </summary>
    private void RefrectProperties()
    {
        try
        {
            //Thời gian tối thiểu giữa 2 lần show ads
            AdsSDK.ins.timeAdsCapping = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("TIME_ADS_CAPPING").LongValue;

            //Chọn mạng để ưu tiên show AOA
            //MAX-YANDEX-ADMOB
            AdsSDK.ins.aoaMediationNetwork = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("aoaMediationNetwork").StringValue;

            //Chọn mạng để ưu tiên show Banner
            //MAX-YANDEX-ADMOB
            AdsSDK.ins.bannerMediationNetwork = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("aoaMediationNetwork").StringValue;

            //Có sử dụng show lần lượt giữa các mediation không
            //True: Các mediation show lần lượt theo thứ tự ưu tiên, cái nào vừa show xong thì ưu tiên thấp nhất
            //Không: Sử dụng mediation có độ ưu tiên cao nhất trước, nếu không có ads thì show ở mediation có độ ưu tiên thấp hơn (độ ưu tiên không thay đổi trong quá trình chơi)
            AdsSDK.ins.isUseMediationTurn = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("isUseMediationTurn").BooleanValue;

            //Danh sách độ ưu tiên của các mediation (**Chỉ áp dụng cho ads Interstital)
            //Nếu độ ưu tiên < 0 tức là tắt mediation đó khỏi danh sách hiển thị
            #region Mediation Priority
            AdsSDK.ins.adsPriority = new List<AdsMediationPriority>();
            var priority_max = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(AdsMediation.MAX.ToString()).LongValue;
            var priority_admob = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(AdsMediation.ADMOB.ToString()).LongValue;
            var priority_yandex = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(AdsMediation.YANDEX.ToString()).LongValue;

            if (priority_max >= 0) AdsSDK.ins.adsPriority.Add(new AdsMediationPriority(AdsMediation.MAX, priority_max));
            if (priority_admob >= 0) AdsSDK.ins.adsPriority.Add(new AdsMediationPriority(AdsMediation.ADMOB, priority_admob));
            if (priority_yandex >= 0) AdsSDK.ins.adsPriority.Add(new AdsMediationPriority(AdsMediation.YANDEX, priority_yandex));
            #endregion

            is_remote_config_success = true;
        }
        catch (Exception ex)
        {
            Debug.Log("Error RefrectProperties: " + ex.Message);
            fetch((bool is_fetch_result) => { });
        }
    }
    #endregion
}

[System.Serializable]
public class KeyData
{
    public static string TIME_ADS_CAPPING = "TIME_ADS_CAPPING";
    public static string IS_SHOW_ADS_IN_FIRST_TIME = "IS_SHOW_ADS_IN_FIRST_TIME";
    public static string ADS_RESUME = "ADS_RESUME";
    //O: INTER
    //1: AOA
    public static string TYPE_ADS_RESUME = "TYPE_ADS_RESUME";
    //public static string ;
}
