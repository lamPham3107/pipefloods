using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FirebaseEvent : MonoBehaviour
{
    public static FirebaseEvent ins;
    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LogEvent(string name)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        FirebaseAnalytics.LogEvent(name);
    }
    #region other
    public void E_levelStart(int level, string mode)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_start", new Parameter[] {
                    new Parameter("level", level.ToString()),
                    new Parameter("mode", mode),
                    new Parameter("day", GameHelper.GetUserLoginDay()),
                });

            }
        }
        catch
        {

        }
    }
    public void E_levelComplete(int level, string mode, int restart)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {

            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_complete", new Parameter[] {
                    new Parameter("level", level.ToString()),
                    new Parameter("mode", mode),
                    new Parameter("day", GameHelper.GetUserLoginDay()),
                    new Parameter("restart",restart),
                });

            }
        }
        catch
        {

        }
    }
    public void E_levelFail(int level, string mode, string reason)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {

            FirebaseAnalytics.LogEvent("level_fail", new Parameter[] {
                    new Parameter("level", level.ToString()),
                    new Parameter("mode", mode),
                    new Parameter("reason", reason),
                });

        }
        catch
        {

        }
    }
    public void E_timePlay(int level, string mode, int times_play)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("time_play", new Parameter[] {
                new Parameter("level", level.ToString()),
                new Parameter("mode", mode),
                new Parameter("times_play", times_play),
            });
            }
        }
        catch
        {
        }
    }
    public void E_openGame(int times)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {
            FirebaseAnalytics.LogEvent("open_game", new Parameter[] {
                new Parameter("times", times.ToString()),
                new Parameter("day_login", GameHelper.GetUserLoginDay()),
                new Parameter("day", "D" + GameHelper.GetDayNow.ToString()),
            });
        }
        catch
        {
        }
    }
    public void E_levelRevive(int level)
    {
        if (!FirebaseManager.ins.is_remote_config_done) return;
        try
        {
            if (!PlayerPrefs.HasKey("first_win_level_" + level))
            {
                FirebaseAnalytics.LogEvent("level_revive", new Parameter[] {
                new Parameter("level", level.ToString()),
                });
            }

        }
        catch
        {
        }
    }
    #endregion
}
