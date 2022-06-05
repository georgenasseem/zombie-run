using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class GameManager : MonoBehaviour
{
    public AudioSource gameplayMusic;
    private PlayerMovement playerScript;

    public GameObject pausePanel;
    public GameObject mainmenuPanel;
    public GameObject revivePanel;
    public GameObject musicButton;
    private GameObject player;
    public GameObject[] skins;

    public Sprite musicOn;
    public Sprite musicOff;

    private Animator pauseAnim;
    private Animator menuAnim;
    private Animator reviveAnim;

    public bool isPaused;
    private bool wifiIsPresent;
    public bool rewardedAd;

    public string music = "on";
    private string shareMessage;
    private string gameURL = "https://play.google.com/store/apps/details?id=com.GeorgeNasseem.Zombierun";
    public string death;

    private void Awake() 
    {
        foreach (GameObject skin in skins)
        {
            skin.SetActive(false);
        }
        skins[PlayerPrefs.GetInt("Skin")].SetActive(true);

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();
        pauseAnim = pausePanel.GetComponent<Animator>();
        menuAnim = mainmenuPanel.GetComponent<Animator>();
        reviveAnim = revivePanel.GetComponent<Animator>();

        pausePanel.SetActive(false);
        mainmenuPanel.SetActive(false);
        revivePanel.SetActive(false);
        isPaused = false;
        death = "";
        StartCoroutine(CheckForWifi());

        if(!PlayerPrefs.HasKey("Music"))
        {
             PlayerPrefs.SetString("Music", music);
        }

        if(PlayerPrefs.GetString("Music") == "on")
        {
            gameplayMusic.Play();
            musicButton.GetComponent<Image>().sprite = musicOn;
            music = "on";
        }
        else
        {
            gameplayMusic.Stop();
            musicButton.GetComponent<Image>().sprite = musicOff;
            music = "off";
        }
    }

    //---------------BUTTONS---------------

    public void Pause()
    {
        if(playerScript.alive && !isPaused)
        {
            isPaused = true;
            pausePanel.SetActive(true);
            pauseAnim.SetTrigger("open");
        }
    }

    public void Unpause()
    {
        if(playerScript.alive)
        {
            isPaused = false;
            pauseAnim.SetTrigger("close");
            playerScript.ChangeDirection();
            playerScript.SlowDown();
        }
    }

    public void Home()
    {
        SaveHighscore();
        AddStars();
        ShowAd();
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        SaveHighscore();
        AddStars();
        ShowAd();
        SceneManager.LoadScene("GamePlay");
    }

    public void ChangeMusic()
    {
        if(PlayerPrefs.GetString("Music") == "on")
        {
            gameplayMusic.Stop();
            musicButton.GetComponent<Image>().sprite = musicOff;
            Debug.Log("off");
            music = "off";
            PlayerPrefs.SetString("Music", music);
        }
        else
        {
            gameplayMusic.Play();
            musicButton.GetComponent<Image>().sprite = musicOn;
            music = "on";
            PlayerPrefs.SetString("Music", music);
        }
    }

    public void Revive()
    {
        rewardedAd = true;
        ShowRewardedAd();
    }

    public void NoRevive()
    {
        reviveAnim.SetTrigger("close");
        playerScript.alive = false;
        playerScript.moveSpeed = 0;
        gameplayMusic.Stop();
        mainmenuPanel.SetActive(true);
        menuAnim.SetTrigger("open");
    }

    public void RevivePlayer()
    {
        playerScript.inviz = true;
        isPaused = false;
        reviveAnim.SetTrigger("close");
        playerScript.SlowDown();
        StartCoroutine("RemoveInviz");
    }

    IEnumerator RemoveInviz()
    {
        yield return new WaitForSeconds(2);
        playerScript.inviz = false;
    }

    //---------------END SCREEN---------------

    public void EndScreen()
    {
        int rand = Random.Range(1, 4);
        if(wifiIsPresent && rand == 1 && death != "Block")
        {
            //revive end screen
            isPaused = true;
            revivePanel.SetActive(true);
            reviveAnim.SetTrigger("open");
        }
        else
        {
            //normal end screen
            SaveHighscore();
            playerScript.DeathAnim();
            playerScript.alive = false;
            playerScript.moveSpeed = 0;
            gameplayMusic.Stop();
            mainmenuPanel.SetActive(true);
            menuAnim.SetTrigger("open");
        }
    }

    void AddStars()
    {
        if(PlayerPrefs.HasKey("Stars"))
        {
            PlayerPrefs.SetInt("Stars", PlayerPrefs.GetInt("Stars") + playerScript.score * 5);
        }
        else
        {
            PlayerPrefs.SetInt("Stars", playerScript.score * 5);
        }
    }

    void SaveHighscore()
    {
        if(PlayerPrefs.HasKey("Highscore"))
        {
            if(PlayerPrefs.GetInt("Highscore") < playerScript.score)
            {
                PlayerPrefs.SetInt("Highscore", playerScript.score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Highscore", playerScript.score);
        }
    }

    //---------------ADS---------------

    void ShowAd()
    {
        if(PlayerPrefs.GetInt("Ads") == 0)
        {
            //Doesn't have AdBlock
            if(PlayerPrefs.HasKey("AdCount"))
            {
                //Not first time playing
                if(PlayerPrefs.GetInt("AdCount") >= 4)
                {
                    //Once every 5 games
                    if(Application.platform != RuntimePlatform.IPhonePlayer)
                    {
                        //Android ad
                        AdsManager.instance.ShowAndroidAd();
                    }
                    else
                    {
                        //Apple ad
                        AdsManager.instance.ShowAppleAd();
                    }
                    PlayerPrefs.SetInt("AdCount", 0);
                }
                else
                {
                    //No ad
                    PlayerPrefs.SetInt("AdCount", PlayerPrefs.GetInt("AdCount") + 1);
                }
            }
            else
            {
                //First time playing
                PlayerPrefs.SetInt("AdCount", 1);
            }
        }
    }

    void ShowRewardedAd()
    {
        if(Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //Android ad
            AdsManager.instance.ShowAndroidRewardedAd();
        }
        else
        {
            //Apple ad
            AdsManager.instance.ShowAppleRewardedAd();
        }
    }

    //---------------SHARE---------------

    public void ClickSharedButton()
    {
        if(Application.platform != RuntimePlatform.IPhonePlayer)
        {
            //Android link
            gameURL = "https://play.google.com/store/apps/details?id=com.GeorgeNasseem.Zombierun";
        }
        else
        {
            //Apple link
            gameURL = "https://play.google.com/store/apps/details?id=com.GeorgeNasseem.Zombierun";
        }

        shareMessage = "I reached speed " + playerScript.speedCounter.ToString() + 
        " in #ZombieRun! My best score is " + PlayerPrefs.GetInt("Highscore").ToString() + 
        ", can you beat me? \n " + gameURL;
        Debug.Log(shareMessage);

        StartCoroutine("Share");
    }

    private IEnumerator Share()
    {
	    yield return new WaitForEndOfFrame();

    	Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
	    ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
	    ss.Apply();

	    string filePath = Path.Combine( Application.temporaryCachePath, "shared img.png" );
	    File.WriteAllBytes( filePath, ss.EncodeToPNG() );

	    // To avoid memory leaks
	    Destroy( ss );

        new NativeShare().AddFile( filePath )
            .SetSubject("Zombie Run").SetText(shareMessage)
		    .SetCallback( ( result, shareTarget ) => Debug.Log(" Share result: " + result + ", selected app: " + shareTarget ) )
		    .Share();
    }

    //---------------WIFI---------------

    IEnumerator CheckForWifi()
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();

        if(request.error != null)
        {
            wifiIsPresent = false;
        }
        else
        {
            wifiIsPresent = true;
        }
    }
}
