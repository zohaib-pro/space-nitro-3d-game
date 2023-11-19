using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    //test id
    //public const string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
    public const string _adUnitId = "ca-app-pub-7329373873254204/2393851787";
    

    public delegate void OnAdload(bool isSuccess);
    public delegate void OnReward();


    private RewardedAd _rewardedAd;
    private OnReward onReward;
    //private GameUiManager gameUiManager;

    // Start is called before the first frame update
    void Start()
    {
        //gameUiManager = GetComponent<GameUiManager>();
        //gameUiManager.Debug("just a test");

        try
        {
            MobileAds.Initialize(initStatus => {});
        }
        catch (Exception e)
        {
            //gameUiManager.Debug("Initialization error: " + e.Message);
        }

    }

    public void LoadRewardedAd(OnAdload onAdload)
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    onAdload(false);
                    return;
                }

                _rewardedAd = ad;
                onAdload(true);
                RegisterEventHandlers(_rewardedAd);
            });
    }

    public void ShowRewardedAd(OnReward onReward)
    {
        this.onReward = onReward; 

        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                onReward();  
            });
        }
        else
        {
            //gameUiManager.Debug("Can't show the aD");
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {

        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {

        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {

        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {

        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {

        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {

        };
    }

}
