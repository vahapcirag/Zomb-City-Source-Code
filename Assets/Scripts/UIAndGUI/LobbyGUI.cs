using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyGUI : MonoBehaviour
{
    [SerializeField] private PlayerNetworkManager playerNetworkManager;
    GUIManager gUIManager;
    [SerializeField] Texture2D riderIcon;
    [SerializeField] Texture2D roboticIcon;

    int screenWidth, screenHeight;

    float x, y;

    string characterName = "";

    bool clicked = false;

    void Start()
    {
        gUIManager = GetComponentInParent<GUIManager>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        x = (float)screenWidth / 1920;
        y = (float)screenHeight / 1080;
    }


    void Update()
    {
        if (playerNetworkManager == null)
        {
            foreach (GameObject playerNetwork in GameObject.FindGameObjectsWithTag("PlayerNetworkManager"))
            {
                playerNetworkManager = playerNetwork.GetComponent<PlayerNetworkManager>();
                if (playerNetworkManager.isLocalPlayer)
                {
                    break;
                }
            }
        }
    }



    private void OnGUI()
    {
        if (playerNetworkManager == null)
            return;

        Rect rider = new Rect(700 * x, 415 * y, 250 * x, 250 * y);
        GUI.Box(rider, riderIcon, gUIManager.title.box);

        if (rider.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown) && (!playerNetworkManager.characterSlot_1))
        {
            clicked = true;
            characterName = "RIDER";
            playerNetworkManager.CmdChangeIndex(1);
            playerNetworkManager.CmdSetCharacter(1);
        }

        Rect robotic = new Rect(1000 * x, 415 * y, 250 * x, 250 * y);
        GUI.Box(robotic, roboticIcon, gUIManager.title.box);

        if (robotic.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown) && (!playerNetworkManager.characterSlot_2))
        {
            clicked = true;
            characterName = "ROBOTIC";
            playerNetworkManager.CmdChangeIndex(2);
            playerNetworkManager.CmdSetCharacter(2);
        }

        Rect layout = new Rect(900 * x, 700 * y, 500 * x, 150 * y);
        GUI.Box(layout, characterName,gUIManager.title.button);


        if (playerNetworkManager.isServer)
        {
            Rect ready = new Rect(900 * x, 900 * y, 500 * x, 150 * y);
            GUI.Box(ready, "Ready",gUIManager.title.button);

            if (ready.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown) && clicked)
            {
                playerNetworkManager.CmdChangeScene();
            }
        }



    }
}
