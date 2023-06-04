using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    [SerializeField] private Banner banner;
    
    private void Awake()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.LogWarning("MobileAds Init!");
        });
        
        banner.LoadAd();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
