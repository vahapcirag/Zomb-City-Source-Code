using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BringToLife : NetworkBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;

        foreach (GameObject item in GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().players)
        {
            if (item!=collision.gameObject && !item.activeSelf)
            {
                RpcF(item);
            }
        }
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    void RpcF(GameObject item)
    {
        item.SetActive(true);
        item.GetComponent<PlayerController>().health = 100;
        item.transform.position = Vector3.zero;
    }
}
