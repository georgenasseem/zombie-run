using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    RectTransform RectTransform;
    MenuManager menuManager;

    void Start()
    {
        RectTransform = GetComponent<RectTransform>();

        if(GameObject.FindGameObjectWithTag("MenuManager") != null)
        {
            menuManager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        }
    }

    public void HidePauseMenu()
    {
        if(GameObject.FindGameObjectWithTag("MenuManager") != null)
        {
            menuManager.UnhideMenu();
        }
        
        gameObject.SetActive(false);
        RectTransform.anchoredPosition = new Vector2(540, 960);
    }
}
