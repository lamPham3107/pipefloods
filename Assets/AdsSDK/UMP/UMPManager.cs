using GoogleMobileAds.Ump.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class UMPManager : MonoBehaviour
{
    [SerializeField]
    private bool _isDebug;

    [SerializeField]
    private List<string> _testDeviceIds;

    public static UMPManager Instance;

    private Action _callback;
    private Action _pauseFunc;
    private Action _resumeFunc;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void InitUMP(Action callback = null, Action pauseFunc = null, Action resumeFunc = null)
    {
        _callback = callback;
        _pauseFunc = pauseFunc;
        _resumeFunc = resumeFunc;

#if UNITY_ANDROID
        Init();
#elif UNITY_IOS
        StartCoroutine(WaitForATTDetermined());
#endif
    }

#if UNITY_IOS
    private IEnumerator WaitForATTDetermined()
    {
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            yield return null;
        }

        Init();
    }
#endif

    private void Init()
    {
        ConsentRequestParameters request;

        if (_isDebug)
        {
            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = _testDeviceIds,
            };

            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
        }
        else
        {
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };
        }

        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            if (_callback != null) _callback();
            return;
        }

        PauseGame();

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError consentError) =>
        {
            ResumeGame();

            if (consentError != null)
            {
                if (_callback != null) _callback();
                return;
            }

            if (ConsentInformation.CanRequestAds())
            {
                if (_callback != null) _callback();
            }
        });
    }

    private void PauseGame()
    {
        if (_pauseFunc != null)
        {
            _pauseFunc();
        }
    }

    private void ResumeGame()
    {
        if (_resumeFunc != null)
        {
            _resumeFunc();
        }
    }
}
