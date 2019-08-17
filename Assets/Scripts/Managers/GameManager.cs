using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public GameObject[] players = new GameObject[2];

    public int level = 1;
    public int scorePerChest_;
    public int fixedScorePerChest_;
    public int totalScore = 0;
    public int server_Score = 0;
    public int p2_Score = 0;

    public int score;

    bool check = false;

    void Start()
    {
       InvokeRepeating("SpawnBox", 1f, 1f);
        InvokeRepeating("FindGuns", 1f, 7f);
        score = 5 * level;
        scorePerChest_ = 350;
        fixedScorePerChest_=350;

        
    }

    private void Update()
    {
        score = 5 * level;
        totalScore = server_Score + p2_Score;
    }


    private void OnLevelWasLoaded(int scene)
    {
        if (scene != 1)
            GetComponent<LobbyManager>().enabled = false;
        if (scene == 2)
            SpawnStats();
    }

  

    
    void SpawnBox()
    {
        if(totalScore>=scorePerChest_)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 2)
                return;
            GameObject go = Instantiate(Resources.Load<GameObject>("BlueBox"),Vector3.zero,Quaternion.identity);
            NetworkServer.Spawn(go);

            scorePerChest_ += fixedScorePerChest_;
        }
    }

   void FindGuns()
    {
        GameObject[] gunList = GameObject.FindGameObjectsWithTag("Gun");

        foreach (GameObject item in gunList)
        {
            
            if (item.transform.parent == null && item.GetComponent<GunValues>().time>7)
                NetworkServer.Destroy(item);
        }
    }
    
    
    void CmdFindPlayers()
    {
        if (GameObject.Find("RoboticUnit(Clone)")!=null)
        {
            players[0] = GameObject.Find("RoboticUnit(Clone)");
        }
        if (GameObject.Find("RiderUnit(Clone)") != null)
        {
            players[1] = GameObject.Find("RiderUnit(Clone)");
        }

    }

   void SpawnStats()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Stats"));
        NetworkServer.Spawn(go);
    }
}
