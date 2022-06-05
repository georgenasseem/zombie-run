using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    public GameManager gameManager;
    public static AdsManager instance;
    string gameID = "";

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Start()
    {
        SetGameID();
        Advertisement.Initialize(gameID);
        Advertisement.AddListener(this);
    }

    public void ShowAndroidAd()
    {
        if(Advertisement.IsReady("Interstitial_Android"))
        {
            Advertisement.Show("Interstitial_Android");
        }
    }

    public void ShowAndroidRewardedAd()
    {
        if(Advertisement.IsReady("Rewarded_Android"))
        {
            Advertisement.Show("Rewarded_Android");
        }
    }

    public void ShowAppleAd()
    {
        if(Advertisement.IsReady("Interstitial_iOS"))
        {
            Advertisement.Show("Interstitial_iOS");
        }
    }

    public void ShowAppleRewardedAd()
    {
        if(Advertisement.IsReady("Rewarded_iOS"))
        {
            Advertisement.Show("Rewarded_iOS");
        }
    }

    public void OnUnityAdsReady(string placementId){}


    public void OnUnityAdsDidError(string message){}

    public void OnUnityAdsDidStart(string placementId){}

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished && gameManager.rewardedAd)
        {
            gameManager.RevivePlayer();
            gameManager.rewardedAd = false;
        }
    }

    private void SetGameID()
    {
        if(Application.platform != RuntimePlatform.IPhonePlayer)
        {
            gameID = "4755319"; //android
        }
        else
        {
            gameID = "4755318"; //apple
        }
    }
}
