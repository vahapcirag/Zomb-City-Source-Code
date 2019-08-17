using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GiveHealth : NetworkBehaviour
{
    int health = 50;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if(collision.gameObject.tag=="Player")
        {
            playerController.health += 50;
            if (playerController.health > 100)
                playerController.health = 100;
            NetworkServer.Destroy(gameObject);
        }
    }
}
