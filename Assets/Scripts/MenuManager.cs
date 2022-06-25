using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;

public class MenuManager : MonoBehaviour
{
    public static MenuManager menuManager;

    public TMP_Text highscoreText;
    public TMP_Text starText;
    public TMP_Text skinText;
    public TMP_Text selectText;
    public TMP_Text Time;
    public Button ClickButton;

    public GameObject selectButton;
    public GameObject otherUI;
    public GameObject musicButton;
    public GameObject settings;
    public GameObject purchasesPanel;
    public GameObject skinsPanel;
    public GameObject starHolder;
    public GameObject menuScene;
    public GameObject player;
    public GameObject rewardsPanel;
    public GameObject restorePurchaseBtn;

    public Sprite musicOn;
    public Sprite musicOff;
    public GameObject[] shopSkins;
    public GameObject[] skins;

    private SpriteRenderer playerRen;
    public AudioSource mainmenuMusic;
    private ulong lastTimeClicked;

    private Animator skinsAnim;
    private Animator settingsAnim;
    private Animator rewardsAnim;
    private Animator purchasesAnim;

    private bool skinsOpened;
    private bool rewardsOpened;
    private bool settingsOpened;
    private bool canCollect;
    private bool wifiIsPresent;

    private string[] skinNames = {"Anna", "Larry", "Halfdan", "Roger", "Uriel", "Victor", "Winzy", "Randall", "Lynch", "Akira", "Darth"};
    public string music = "on";
    private string purchaseRemoveAds = "com.georgenasseem.zombierun.removeads";
    private string purchase1kstars = "com.georgenasseem.zombierun.2500stars";
    private string purchase5kstars = "com.georgenasseem.zombierun.10000stars";

    public int rewardAmount = 100;
    private int skinIndex;
    public float msToWait;

    //android ad id: 4755319
    //apple ad id: 4755318
    //normal ad ios: Interstitial_iOS
    //normal ad android: Interstitial_Android
    //rewarded ad ios: Rewarded_iOS
    //rewarded ad android: Rewarded_Android

    private void Awake() 
    {
        HideRestorePurchaseBtn();

        otherUI.SetActive(true);
        menuScene.SetActive(true);
        starHolder.SetActive(true);
        skinsPanel.SetActive(false);
        settings.SetActive(false);
        rewardsPanel.SetActive(false);
        purchasesPanel.SetActive(false);

        skinsAnim = skinsPanel.GetComponent<Animator>();
        settingsAnim = settings.GetComponent<Animator>();
        rewardsAnim = rewardsPanel.GetComponent<Animator>();
        purchasesAnim = purchasesPanel.GetComponent<Animator>();

        ChangeMenuSprite();
        PlayerPrefs.SetString("0", "Buy");

        if(!PlayerPrefs.HasKey("FirstTime"))
        {
            PlayerPrefs.SetString("FirstTime", "Yes");
            Click();
        }

        if(PlayerPrefs.HasKey("Highscore"))
        {
            highscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
        }

        if(PlayerPrefs.HasKey("Stars"))
        {
            starText.text = PlayerPrefs.GetInt("Stars").ToString();
        }

        if(PlayerPrefs.HasKey("Skin"))
        {
            skinIndex = PlayerPrefs.GetInt("Skin");
        }
        else
        {
            PlayerPrefs.SetInt("Skin", 0);
        }

        if(PlayerPrefs.GetString("Music") == "on")
        {
            mainmenuMusic.Play();
            musicButton.GetComponent<Image>().sprite = musicOn;
            music = "on";
        }
        else
        {
            mainmenuMusic.Stop();
            musicButton.GetComponent<Image>().sprite = musicOff;
            music = "off";
        }
    }

    private void Start()
	{
        if(PlayerPrefs.HasKey("LastTimeClicked"))
        {
            lastTimeClicked = ulong.Parse(PlayerPrefs.GetString("LastTimeClicked"));
    
            if (!Ready())
            {
                ClickButton.interactable = false;
            }
        }
        else
        {
            lastTimeClicked = (ulong)DateTime.Now.Ticks;
			PlayerPrefs.SetString("LastTimeClicked", lastTimeClicked.ToString());
        }

        starText.text = PlayerPrefs.GetInt("Stars").ToString();
	}

    private void Update()
    {
        if (!ClickButton.IsInteractable())
        {
            if (Ready())
            {
                ClickButton.interactable = true;
                Time.text = "Ready!";
                return;
            }
            ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (float)(msToWait - m) / 1000.0f;
    
            string r = "";
            //HOURS
            r += ((int)secondsLeft / 3600).ToString() + ":";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            //MINUTES
            r += ((int)secondsLeft / 60).ToString("00") + ":";
            //SECONDS
            r += (secondsLeft % 60).ToString("00");
            Time.text = r;
        }
    }

    //--------------BUTTONS--------------

    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ChangeMusic()
    {
        if(PlayerPrefs.GetString("Music") == "on")
        {
            mainmenuMusic.Stop();
            musicButton.GetComponent<Image>().sprite = musicOff;
            music = "off";
            PlayerPrefs.SetString("Music", music);
        }
        else
        {
            mainmenuMusic.Play();
            musicButton.GetComponent<Image>().sprite = musicOn;
            music = "on";
            PlayerPrefs.SetString("Music", music);
        }
    }

    //--------------OPEN AND CLOSE PANELS--------------

    public void Settings()
    {
        otherUI.SetActive(false);
        menuScene.SetActive(false);
        settings.SetActive(true);
        starHolder.SetActive(false);
        settingsAnim.SetTrigger("open");
        settingsOpened = true;
        HideMainChar();
    }

    public void Skins()
    {
        otherUI.SetActive(false);
        menuScene.SetActive(false);
        skinsPanel.SetActive(true);
        skinsAnim.SetTrigger("open");
        skinsOpened = true;
        HideMainChar();
        ShowShopSkins();
    }

    public void Gift()
    {
        otherUI.SetActive(false);
        menuScene.SetActive(false);
        rewardsPanel.SetActive(true);
        starHolder.SetActive(true);
        rewardsAnim.SetTrigger("open");
        rewardsOpened = true;
        HideMainChar();
    }

    public void Purchases()
    {
        otherUI.SetActive(false);
        menuScene.SetActive(false);
        purchasesPanel.SetActive(true);
        starHolder.SetActive(true);
        purchasesAnim.SetTrigger("open");
        HideMainChar();
    }

    public void Back()
    {
        if(skinsOpened)
        {
            skinsAnim.SetTrigger("close");
        }
        else if(settingsOpened)
        {
            settingsAnim.SetTrigger("close");
        }
        else if(rewardsOpened)
        {
            rewardsAnim.SetTrigger("close");
        }
        else
        {
            purchasesAnim.SetTrigger("close");
        }
    }

    public void UnhideMenu()
    {
        otherUI.SetActive(true);
        menuScene.SetActive(true);
        starHolder.SetActive(true);
        ChangeMenuSprite();

        if(skinsOpened)
        {
            skinsOpened = false;
            //skinsPanel.SetActive(false);
            skinsAnim.SetTrigger("close");
        }
        else if(settingsOpened)
        {
            settingsOpened = false;
            //settings.SetActive(false);
            settingsAnim.SetTrigger("close");
        }
        else if(rewardsOpened)
        {
            rewardsOpened = false;
            //rewardsPanel.SetActive(false);
            rewardsAnim.SetTrigger("close");
        }
        else
        {
            //purchasesPanel.SetActive(false);
            purchasesAnim.SetTrigger("close");
        }
    }

    //--------------SKINS--------------

    public void RightArrow()
    {
        skinIndex++;
        if(skinIndex == 11)
        {
            skinIndex = 0;
        }
        ShowShopSkins();
    }

    public void LeftArrow()
    {
        if(skinIndex == 0)
        {
            skinIndex = 11;
        }
        skinIndex--;
        ShowShopSkins();
    }

    public void SelectSkin()
    {
        if(PlayerPrefs.HasKey(skinIndex.ToString()))
        {
            PlayerPrefs.SetInt("Skin", skinIndex);
        }
        else if(Int32.Parse(starText.text) >= 1500)
        {
            PlayerPrefs.SetInt("Skin", skinIndex);
            PlayerPrefs.SetString(skinIndex.ToString(), "Buy");
            PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") - 1500);
            starText.text = PlayerPrefs.GetInt("Stars").ToString();
            selectText.text = "Select?";
        }
        CheckIfSelected();
    }

    private void ShowShopSkins()
    {
        foreach (GameObject skin in shopSkins)
        {
            skin.SetActive(false);
        }

        if(PlayerPrefs.HasKey(skinIndex.ToString()))
        {
            selectText.text = "Select?";
        }
        else
        {
            selectText.text = "1500 stars";
        }

        skinText.text = skinNames[skinIndex];
        shopSkins[skinIndex].SetActive(true);
        CheckIfSelected();
    }

    void CheckIfSelected()
    {
        if(PlayerPrefs.GetInt("Skin") == skinIndex)
        {
            selectButton.GetComponent<Image>().color = new Color32(127,136,253,100);
        }
        else
        {
            selectButton.GetComponent<Image>().color = new Color32(255,255,225,100);
        }
    }

    void ChangeMenuSprite()
    {
        foreach (GameObject skin in skins)
        {
            skin.SetActive(false);
        }

        skins[PlayerPrefs.GetInt("Skin")].SetActive(true);
    }

    void HideMainChar()
    {
        foreach (GameObject skin in skins)
        {
            skin.SetActive(false);
        }
    }

    //--------------DAILY REWARD--------------
    
    public void Click()
    {
        lastTimeClicked = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastTimeClicked", lastTimeClicked.ToString());
        ClickButton.interactable = false;

        if(canCollect)
        {
            canCollect = false;
            PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + rewardAmount);
            starText.text = PlayerPrefs.GetInt("Stars").ToString();
        }
    }

    private bool Ready()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
     
        float secondsLeft = (float)(msToWait - m) / 1000.0f;
     
        if (secondsLeft < 0)
        {
            canCollect = true;
            return true;
        }
    
        return false;
    }

    //--------------PURCHASING--------------

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("Ads", 1);
    }

    public void Buy25000stars()
    {
        PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + 2500);
        starText.text = PlayerPrefs.GetInt("Stars").ToString();
    }

    public void Buy10000stars()
    {
        PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + 10000);
        starText.text = PlayerPrefs.GetInt("Stars").ToString();
    }

    public void OnPurchaseComplete(Product product)
    {
        if(product.definition.id == purchaseRemoveAds)
        {
            RemoveAds();
        }
        else if(product.definition.id == purchase1kstars)
        {
            Buy25000stars();
        }
        else if(product.definition.id == purchase5kstars)
        {
            Buy10000stars();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of " + product.definition.id + " failed due to " + reason);
    }

    private void HideRestorePurchaseBtn()
    {
        if(Application.platform != RuntimePlatform.IPhonePlayer)
        {
            restorePurchaseBtn.SetActive(false);
        }
    }
}

     
          
                        