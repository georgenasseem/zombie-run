using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMaker : MonoBehaviour
{
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private Transform leftPos, rightPos;
    [SerializeField] private Transform[] initialPos;

    public GameManager gameManager;
    private GameObject spawnedMonsterL;
    private GameObject spawnedMonsterR;
    private Vector3 spawnPos;
    public int spaceBetweenEnemies = 11;
    private int randomIndex;
    private int randomSide;
    private int oldRandomSide;
    private bool firstLap = true;
    private int minTimeForSpawn = 2, maxTimeForSpawn = 5;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(nameof(SpawnMonsters));
    }

    private IEnumerator SpawnMonsters()
    {
        for (int i = 0; i < initialPos.Length; i++)
        {
            randomIndex = Random.Range(0, monsters.Length);

            if (initialPos[i].transform.position.x < 0)
            {
                spawnedMonsterL = Instantiate(monsters[randomIndex]);
                spawnedMonsterL.transform.position = initialPos[i].transform.position;
                spawnedMonsterL.GetComponent<Monster>().speed = 2.7f;
                spawnedMonsterL.transform.localScale = new Vector3(1.66f, 1.66f, 1f);
            }
            else
            {
                spawnedMonsterR = Instantiate(monsters[randomIndex]);
                spawnedMonsterR.transform.position = initialPos[i].transform.position;
                spawnedMonsterR.GetComponent<Monster>().speed = -2.7f;
                spawnedMonsterR.transform.localScale = new Vector3(-1.66f, 1.66f, 1f);
            }
        }

        while (!gameManager.isPaused)
        {
            yield return new WaitForSeconds(Random.Range(minTimeForSpawn, maxTimeForSpawn));

            if(InDistance())
            {
                randomIndex = Random.Range(0, monsters.Length);

                if (randomSide == 0)
                {
                    spawnedMonsterL = Instantiate(monsters[randomIndex]);
                    spawnedMonsterL.transform.position = leftPos.position;
                    spawnedMonsterL.GetComponent<Monster>().speed = 2.7f;
                    spawnedMonsterL.transform.localScale = new Vector3(1.66f, 1.66f, 1f);
                }
                else
                {
                    spawnedMonsterR = Instantiate(monsters[randomIndex]);
                    spawnedMonsterR.transform.position = rightPos.position;
                    spawnedMonsterR.GetComponent<Monster>().speed = -2.7f;
                    spawnedMonsterR.transform.localScale = new Vector3(-1.66f, 1.66f, 1f);
                }
            }
        }
   }

   private bool InDistance()
   {
       randomSide = Random.Range(0, 2);
       spaceBetweenEnemies = Random.Range(12, 16);

       if(randomSide == 0)
       {
           if(Mathf.Abs(leftPos.position.x - spawnedMonsterL.transform.position.x) > spaceBetweenEnemies)
           {
               return true;
           }
           else
           {
               //Debug.Log("Left:" + Mathf.Abs(leftPos.position.x - spawnedMonsterL.transform.position.x));
               return false;
           }
       }
       else
       {
           if(Mathf.Abs(rightPos.position.x - spawnedMonsterR.transform.position.x) > spaceBetweenEnemies)
           {
               return true;
           }
           else
           {
               Debug.Log("Right:" + Mathf.Abs(rightPos.position.x - spawnedMonsterR.transform.position.x));
               return false;
           }
       }
   }
}
