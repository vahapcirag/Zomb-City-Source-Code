using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    public short damage;
    string path;

    Vector2 direction;

    public GameObject player;
    GameObject obstacle;

    float lifeTime = 2f, time;

    GameManager gameManager;

    bool check=false;

   

    private void Update()
    {


        if (!isServer)
            return;

        if(!check)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            check = true;
        }

        time += Time.deltaTime;
        if (time > lifeTime)
        {
            CmdDestroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<ZombieController>().TakeDamage(player, damage);
        }
            
        else if (collision.gameObject.tag == "BlueBox")
        {
            CmdSpawn(collision.gameObject.transform.position);
            CmdDestroy(collision.gameObject);
        }
        CmdDestroy(gameObject);
    }

    [Command]
    void CmdDestroy(GameObject go)
    {
        NetworkServer.Destroy(go);
    }

    [Command]
    void CmdSpawn(Vector3 pos)
    {
        GameObject deadPlayer=null;
        Debug.Log(33);

        foreach (GameObject item in GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().players)
        {
            if (!item.activeSelf)
            {
                deadPlayer = item;
                break;
            }
        }

        Debug.Log(deadPlayer);

        string path;

        int rand = Random.Range(0,6);

        if(deadPlayer!=null)
        {
            path = "YellowHearth";
        }
        else if  (rand <3)
        {
            path = "Hearth_";
        }
        else if(rand==4)
        {
            path = "Guns/Guitar";
        }
        else if (rand == 5)
        {
            path = "Guns/ElectroLigthGun";
        }
        else
        {
            path = "Guns/M4A1";
        }
        GameObject go = Instantiate(Resources.Load<GameObject>(path), pos, Quaternion.identity);
        NetworkServer.Spawn(go);
    }

    [ClientRpc]
    void RpcSpawn(GameObject go)
    {
        go.transform.position = Resources.Load<GameObject>(path).transform.position;
        go.transform.localScale = Resources.Load<GameObject>(path).transform.lossyScale;
    }

}
