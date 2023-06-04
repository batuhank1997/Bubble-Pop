using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif
    
    [SerializeField] private Banner banner;
    [SerializeField] private Intersitatial intersitatial;
    
    private void Awake()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.LogWarning("MobileAds Init!");
        });
        
        banner.LoadAd();
        intersitatial.LoadInterstitialAd();
    }
    
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            intersitatial.ShowAd();
        }
    }
}
