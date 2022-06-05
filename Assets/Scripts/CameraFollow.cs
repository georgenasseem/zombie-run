using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject[] players;
    private Transform playerPos;
    private Vector3 tempPos;

    private void Awake() 
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        playerPos = players[PlayerPrefs.GetInt("Skin")].transform;
    }

    private void LateUpdate()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (!playerPos)
        {
            playerPos = GameObject.FindWithTag("Player").transform;
            return;
        }

        if(!(playerPos.position.x >= 54.8 || playerPos.position.x <= -54.65))
        {
            tempPos = transform.position;
            tempPos.x = playerPos.position.x;
            transform.position = tempPos;
        }
    }
} // class


























