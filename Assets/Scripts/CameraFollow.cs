using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject[] players;
     [SerializeField] GameObject myPlayer;
    public Rigidbody2D rb;


    void Start()
    {
        Invoke("FindPlayers", 0.5f);
        //InvokeRepeating("Move", 0, 0.005f);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (myPlayer == null)
            return;
        Vector3 movePosition = myPlayer.GetComponent<Rigidbody2D>().position;
        movePosition.z -= 10;
        movePosition = Vector3.Lerp(rb.position, movePosition, 0.3f);
        rb.position = movePosition;
    }

    void FindPlayers()
    {
         players = GameObject.FindGameObjectsWithTag("Player");
        int i;

        for (i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().hasAuthority == true)
            {
                break;
            }
        }

        myPlayer = players[i];
    }
}
