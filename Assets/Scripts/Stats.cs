using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Stats : NetworkBehaviour
{
    [SerializeField] GameObject[] players;
    GameObject p1Score;
    GameObject p1Health;
    GameObject p2Score;
    GameObject p2Health;
    GameObject totalScore_;
    GameObject zombieScore;
    GameObject level;



    [SerializeField] bool oneTime2 = false;

    private void Start()
    {
        p1Score = GameObject.FindGameObjectWithTag("P1Score");
        p2Score = GameObject.FindGameObjectWithTag("P2Score");
        p1Health = GameObject.FindGameObjectWithTag("P1Health");
        p2Health = GameObject.FindGameObjectWithTag("P2Health");
        totalScore_ = GameObject.FindGameObjectWithTag("TotalScore");
        zombieScore = GameObject.FindGameObjectWithTag("ZombScore");
        level = GameObject.FindGameObjectWithTag("Level");
        Invoke("FindPlayers", 1f);
    }

    private void Update()
    {
        if (!oneTime2 && hasAuthority)
        {
            InvokeRepeating("CmdFindStats", 0.5f, 0.1f);
            oneTime2 = true;
        }
    }

    [Command]
    void CmdFindStats()
    {
        if (!isServer)
            return;

        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        ZombieManager zombieManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieManager>();

        if (GameObject.FindGameObjectsWithTag("Player").Length != 1)
        {

            RpcGiveClientsInformation(gameManager.server_Score, gameManager.p2_Score,
                0, zombieManager.zombieCount - zombieManager.deadZombieCount, gameManager.level);
        }
        else
        {
            RpcGiveClientsInformation(gameManager.server_Score, gameManager.p2_Score,
                0f, zombieManager.zombieCount - zombieManager.deadZombieCountOncurrentLevel, gameManager.level);
        }


    }

    [ClientRpc]
    void RpcGiveClientsInformation(int sScore, int oScore, float oHealth, int beforeLevelZombie, int level_)
    {
        if (isServer)
        {
            p1Score.GetComponent<Text>().text = sScore.ToString();
            p2Score.GetComponent<Text>().text = oScore.ToString();
            if (players.Length==0)
                return;
            p1Health.GetComponent<Text>().text = players[0].GetComponent<PlayerController>().health.ToString();
            if (!(players.Length==1))
                p2Health.GetComponent<Text>().text = players[1].GetComponent<PlayerController>().health.ToString();
            totalScore_.GetComponent<Text>().text = (sScore + oScore).ToString();
            zombieScore.GetComponent<Text>().text = beforeLevelZombie.ToString();
            level.GetComponent<Text>().text = level_.ToString();
        }
        else
        {
            p2Score.GetComponent<Text>().text = sScore.ToString();
            p1Score.GetComponent<Text>().text = oScore.ToString();
            if (players.Length == 0)
                return;
            p2Health.GetComponent<Text>().text = players[0].GetComponent<PlayerController>().health.ToString();
            if (!(players.Length == 1))
                p1Health.GetComponent<Text>().text = players[1].GetComponent<PlayerController>().health.ToString();
            totalScore_.GetComponent<Text>().text = (sScore + oScore).ToString();
            zombieScore.GetComponent<Text>().text = beforeLevelZombie.ToString();
            level.GetComponent<Text>().text = level_.ToString();
        }
    }

    void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
}
