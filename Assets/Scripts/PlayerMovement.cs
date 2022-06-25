using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement playerMovement;

    [SerializeField]
    public float moveSpeed = 7f, jumpForce = 20f;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text speedText;

    public GameManager gameManager;
    private GameObject playerHolder;
    public GameObject animObject;
    private Rigidbody2D myBody;

    private AnimObject anim;
    private Animator textAnim;
    public AudioSource jumpSound;
    public AudioSource deathSound;

    private int speedTimer;
    public int score;
    public int speedCounter = 1;

    private bool canJump;
    private bool tempPass;
    private bool canGetPoint;
    public bool inviz;
    public bool alive = true;

    private float currentXScale = 0.011f;
    private float oldCurrentXScale = 0.011f;
    private float timeClicked;
    private float tempSpeed;
    private float timeBetweenClicks = 0.24f;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerHolder = GameObject.FindGameObjectWithTag("PlayerHolder");
        myBody = GetComponent<Rigidbody2D>();

        anim = animObject.GetComponent<AnimObject>();
        textAnim = scoreText.GetComponent<Animator>();
        speedText.text = "Speed: 1";
    }

    private void Update()
    {
        if(alive && !gameManager.isPaused)
        {
            CheckInput();
        }
    }
    
    private void FixedUpdate()
    {
        if(alive && !gameManager.isPaused)
        {
            MovePlayer();
            IncreaseMove();
        }
    }

    void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) || tempPass)
        {
            if(!tempPass)
            {
                timeClicked = Time.time;
            }

            tempPass = true;
            
            if(Input.GetKeyDown(KeyCode.Mouse0) && timeClicked != Time.time)
            {
                tempPass = false;
                PlayerJump(); //double
            }

            if(Time.time > timeClicked + timeBetweenClicks)
            {
                tempPass = false;
                ChangeDirection(); //single
            }
        }
    }

    void MovePlayer()
    {
        myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);
    }

    void IncreaseMove()
    {
        speedTimer += 1;
        if(speedTimer == 550 && speedCounter < 9)
        {
            speedCounter++;
            speedText.text = "Speed: " + speedCounter.ToString();

            if(moveSpeed > 0)
            {
                moveSpeed += 0.75f;
            }
            else
            {
                moveSpeed -= 0.75f;
            }
            
            speedTimer = 0;
        }
    }

    void PlayerJump()
    {
        if (canJump)
        {
            if(PlayerPrefs.GetString("Music") == "on")
            {
                jumpSound.Play();
            }

            canJump = false;
            myBody.velocity = Vector2.zero;
            myBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            anim.CheckJump(canJump);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Block" && !inviz)
        {
            gameManager.death = "Block";
            Die();

            if(PlayerPrefs.GetString("Music") == "on")
            {
                deathSound.Play();
            }
        }

        if(other.gameObject.tag == "Enemy" && !inviz)
        {
            if(PlayerPrefs.GetString("Music") == "on")
            {
                deathSound.Play();
            }

            Die();
        }

        if(other.gameObject.tag == "Ground")
        {
            canJump = true;
            canGetPoint = true;
            anim.playJumpAnim = true;
            anim.CheckJump(canJump);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy" && canGetPoint)
        {
            canGetPoint = false;
            score++;
            scoreText.text = score.ToString();
            textAnim.SetTrigger("Move");
        }
    }

    public void ChangeDirection()
    {
        transform.localScale =  new Vector3(-currentXScale, 0.011f, 0.011f);
        currentXScale *= -1;
        moveSpeed = -moveSpeed;
    }

    void Die()
    {
        if(alive)
        {
            gameManager.EndScreen();
        }
    }

    public void DeathAnim()
    {
        anim.Death();
    }

    public void SlowDown()
    {
        tempSpeed = moveSpeed;
        oldCurrentXScale = currentXScale;
        if(currentXScale > 0)
        {
            moveSpeed = 1;
        }else
        {
            moveSpeed = -1;
        }
        speedText.text = "Speed: 1";
        StartCoroutine("RevertSpeed");
    }

    IEnumerator RevertSpeed()
    {
        yield return new WaitForSeconds(1);

        if(currentXScale == oldCurrentXScale)
        {
            moveSpeed = tempSpeed;
        }else
        {
            moveSpeed = -tempSpeed;
        }
        speedText.text = "Speed: " + speedCounter.ToString();
    }
}



