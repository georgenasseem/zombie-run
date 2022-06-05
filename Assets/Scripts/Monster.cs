using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [HideInInspector] public float speed;

    private Rigidbody2D _rb;
    public GameManager gameManager;
    private GameObject player;
    private PlayerMovement playerScript;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!gameManager.isPaused && playerScript.alive)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
        }
    }
}