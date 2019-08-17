using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ZombieManager : NetworkBehaviour
{
    bool check = false;

    Transform spawn;

    [SerializeField ]GameObject[] spawnAreas =new GameObject[12];

    GameManager gameManager;

    
    public int maxZombiCount;
    public int zombieCount;
    public int zombieHealt;
    public int zombieDamage;
    public int specialZombieDamage;
    public int deadZombieCountOncurrentLevel;
    public int deadZombieCount = 0;

    bool checkLevel;

    private void Start()
    {
       
        InvokeRepeating("ZombieSpawner", 2f, .5f);
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {


        if (!checkLevel)
        {
            zombieCount = 0;
            maxZombiCount = gameManager.level * 2 + 3;
            zombieHealt = gameManager.level / 2 + 8;
            zombieDamage = gameManager.level / 2 + 8;
            deadZombieCountOncurrentLevel = 0;
            checkLevel = true;
        }

        if (deadZombieCountOncurrentLevel == maxZombiCount)
        {
            gameManager.level++;
            checkLevel = false;
            deadZombieCountOncurrentLevel = 0;
        }

        if (SceneManager.GetActiveScene().buildIndex == 2 && !check)
        {
            spawn = GameObject.Find("SpawnAreas").transform;

            for (int i = 0; i < spawn.childCount-1; i++)
            {
                spawnAreas[i] = spawn.GetChild(i).gameObject;
            }

            check = true;
        }
    }

    void ZombieSpawner()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2 || (zombieCount >= maxZombiCount))
            return;

        int rand=Random.Range(0,6);
        string path="";

        if (rand == 0)
            path = "FatZombie/FatZombieUnit";
        else if (rand >= 1 && rand < 4)
            path = "AuntZombie/AuntZombieUnit";
        else if (rand >= 4)
            path = "ManZombie/ManZombieUnit";
        GameObject spawnArea = spawnAreas[Random.Range(0, 12)];
        

        GameObject gO = Instantiate(Resources.Load<GameObject>("Zombies/"+path),spawnArea.transform.position,Quaternion.identity);
        NetworkServer.Spawn(gO);
        zombieCount++;
    }

    


}
