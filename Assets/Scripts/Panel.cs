using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    MenuManager menuManager;

    void Start()
    {
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
    }
}
