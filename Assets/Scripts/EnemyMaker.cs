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
    private int randomIndex;
    private int randomSide;
    private int oldRandomSide;
    private bool firstLap = true;
    private int minTimeForSpawn = 2, maxTimeForSpawn = 4;

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
                spawnedMonsterL.GetComponent<Monster>().speed = 2f;
                spawnedMonsterL.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
            else
            {
                spawnedMonsterR = Instantiate(monsters[randomIndex]);
                spawnedMonsterR.transform.position = initialPos[i].transform.position;
                spawnedMonsterR.GetComponent<Monster>().speed = -2f;
                spawnedMonsterR.transform.localScale = new Vector3(-1.2f, 1.2f, 1f);
            }
        }

        while (!gameManager.isPaused)
        {
            yield return new WaitForSeconds(Random.Range(minTimeForSpawn, maxTimeForSpawn));
            randomSide = Random.Range(0, 2);

            if(InDistance())
            {
                randomIndex = Random.Range(0, monsters.Length);

                if (randomSide == 0)
                {
                    spawnedMonsterL = Instantiate(monsters[randomIndex]);
                    spawnedMonsterL.transform.position = leftPos.position;
                    spawnedMonsterL.GetComponent<Monster>().speed = 2f;
                    spawnedMonsterL.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                }
                else
                {
                    spawnedMonsterR = Instantiate(monsters[randomIndex]);
                    spawnedMonsterR.transform.position = rightPos.position;
                    spawnedMonsterR.GetComponent<Monster>().speed = -2f;
                    spawnedMonsterR.transform.localScale = new Vector3(-1.2f, 1.2f, 1f);
                }
            }
        }
   }

   private bool InDistance()
   {
       if(firstLap)
       {
           firstLap = false;
           return true;
       }

       if(randomSide == 0)
       {
           if(Mathf.Abs(leftPos.position.x - spawnedMonsterL.transform.position.x) > 11)
           {
               return true;
           }
           else
           {
               return false;
           }
       }
       else
       {
           if(Mathf.Abs(rightPos.position.x - spawnedMonsterR.transform.position.x) > 11)
           {
               return true;
           }
           else
           {
               return false;
           }
       }
   }
}
