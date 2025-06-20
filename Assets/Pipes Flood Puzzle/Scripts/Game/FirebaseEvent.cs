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
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent(name);
        }

    }
    #region other
    public void E_levelStart(int level, string mode)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            try
            {

                if (!PlayerPrefs.HasKey("first_win_level_" + level))
                {
                    FirebaseAnalytics.LogEvent("level_start", new Parameter[] {
                    new Parameter("level", level.ToString()),
                    new Parameter("mode", mode),
                    new Parameter("day", GameHelper.GetUserLoginDay()),
                });
                    Debug.Log("Log level_start event for level: " + level + ", mode: " + mode);
                }
            }
            catch
            {

            }
        }

    }
    public void E_levelComplete(int level, string mode, int restart)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            try
            {

                if (!PlayerPrefs.HasKey("first_win_level_" + level ))
                {
                    FirebaseAnalytics.LogEvent("level_complete", new Parameter[] {
                    new Parameter("level", level.ToString()),
                    new Parameter("mode", mode),
                    new Parameter("day", GameHelper.GetUserLoginDay()),
                    new Parameter("restart",restart),
                });
                    Debug.Log("Log level_complete event for level: " + level + ", mode: " + mode + ", restart: " + restart);
                }
            }
            catch
            {

            }
        }

    }
    public void E_levelFail(int level, string mode, string reason)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
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

    }
    public void E_timePlay(int level, string mode, int times_play)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            try
            {
                FirebaseAnalytics.LogEvent("time_play", new Parameter[] {
                new Parameter("level", level.ToString()),
                new Parameter("mode", mode),
                new Parameter("times_play", times_play),
            });
                
            }
            catch
            {
            }
        }

    }
    public void E_openGame(int times, int maxMissionID , int maxLevelID)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            try
            {
                FirebaseAnalytics.LogEvent("open_game", new Parameter[] {
                new Parameter("times", times.ToString()),
                new Parameter("day_login", GameHelper.GetUserLoginDay()),
                new Parameter("time_block", GameHelper.GetBlockTime()),
                new Parameter("max_mission" , maxMissionID.ToString() ),
                new Parameter("max_level" , maxLevelID.ToString() ),
            });
                Debug.Log("Log open_game event with times: " + times);
            }
            catch
            {
            }
        }

    }
    public void E_levelRevive(int level)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
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

    }
    #endregion
}
