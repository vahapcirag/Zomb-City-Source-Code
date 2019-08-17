using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManuGUISystem : MonoBehaviour
{
    private CustomNetworkManager networkManager;

    private GUIManager gUIManager;

    private GameObject myPlayer;

    float x, y; //The x and y is ratio
    byte k, j;
    bool isFullScreen;
    bool vSync;

    bool check = false;

    void Start()
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        gUIManager = GetComponentInParent<GUIManager>();


        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].height == Screen.height)
            {

                j = (byte)i;
                break;
            }
        }

        isFullScreen = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 1)
        {
            vSync = true;
        }
        else
            vSync = false;
    }

    private void Update()
    {
        FindPlayers();

        if (Input.GetKeyDown(KeyCode.K))
        {
            networkManager.hostAdress = "192.168.1.14";
            networkManager.JoinGame();
        }

        x = gUIManager.x;
        y = gUIManager.y;
    }

    private void OnGUI()
    {
        MainGUIElements();
        MainSettingsMenu();
        GraphicsSettingsMenu();
    }

    void MainGUIElements()
    {
        GUI.Box(new Rect(100 * x, 175 * y, 1500 * x, 250 * y), "The Zomb City", gUIManager.title.box);


        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
        {
            Rect hostButton = new Rect(250 * x, 500 * y, 500 * x, 100 * y);
            GUI.Box(hostButton, "Play as Host", gUIManager.title.button);

            if (hostButton.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
            {
                networkManager.StartAsHost(check);

            }

            Rect searchMatchButton = new Rect(250 * x, 625 * y, 500 * x, 100 * y);
            GUI.Box(searchMatchButton, "Find Match", gUIManager.title.button);

            if (searchMatchButton.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
            {
                check = true;
                networkManager.FindGame();
            }
        }
        else
        {
            Rect disconnetButton = new Rect(250 * x, 625 * y, 500 * x, 100 * y);
            GUI.Box(disconnetButton, "Disconnect", gUIManager.title.button);

            if (disconnetButton.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
            {
                if (myPlayer.GetComponent<PlayerNetworkManager>().isServer)
                    networkManager.ShutDownServer();
                networkManager.Disconnect();
            }
        }


        Rect settingsButton = new Rect(250 * x, 750 * y, 500 * x, 100 * y);
        GUI.Box(settingsButton, "Settings", gUIManager.title.button);

        if (settingsButton.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
            k = 1;


        Rect creditsButton = new Rect(250 * x, 875 * y, 500 * x, 100 * y);
        GUI.Button(creditsButton, "Credits", gUIManager.title.button);

        for (int i = 0; i < networkManager.gameAdresses.Count; i++)
        {
            Rect gameList = new Rect(1000 * x, 875 * y * i, 500 * x, 100 * y);
            GUI.Box(gameList, networkManager.gameAdresses[i], gUIManager.title.textField);

            if (gameList.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
            {
                networkManager.hostAdress = networkManager.gameAdresses[i];
                networkManager.JoinGame();
            }
        }
    }

    void MainSettingsMenu()
    {
        if (k != 1)
            return;

        for (int i = 1; i < 4; i++)
        {
            Rect mainSettingsRect = new Rect(1400 * x, 50 + 200 * y * i, 1000 * x, 100 * y);

            switch (i)
            {
                case 1:
                    GUI.Box(mainSettingsRect, "Graphics", gUIManager.title.button);

                    if (mainSettingsRect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
                        k = 2;
                    break;
                case 2:
                    GUI.Box(mainSettingsRect, "Controller", gUIManager.title.button);
                    if (mainSettingsRect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
                        k = 3;
                    break;
                case 3:
                    GUI.Box(mainSettingsRect, "Sound", gUIManager.title.button);
                    if (mainSettingsRect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
                        k = 4;
                    break;

            }
        }
    }

    void GraphicsSettingsMenu()
    {
        if (k != 2)
            return;

        Resolution[] resolutions = Screen.resolutions;


        for (int i = 1; i < 6; i++)
        {
            Rect graphicsSettingsRect = new Rect(1400 * x, -110 * y + 240 * y * i, 1000 * x, 100 * y);

            switch (i)
            {
                case 1:

                    GUI.Box(graphicsSettingsRect, "Resolution", gUIManager.title.button);

                    Rect leftArrow = new Rect(1300 * x, 210 * y, 100 * x, 100 * y);
                    GUI.Box(leftArrow, "<"/*, title.button*/);
                    if (leftArrow.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown) && (j > 0))
                        j--;

                    Rect rightArrow = new Rect(1735 * x, 210 * y, 100 * x, 100 * y);
                    GUI.Box(rightArrow, "<"/*, title.button*/);
                    if (rightArrow.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown) && (j < Screen.resolutions.Length - 1))
                        j++;

                    Rect resolutionsRect = new Rect(1050 * x, 210 * y, 1000 * x, 100 * y);
                    GUI.Box(resolutionsRect, resolutions[j].ToString(), gUIManager.title.label);
                    Debug.Log(j);
                    break;
                case 2:
                    GUI.Box(graphicsSettingsRect, "Fullscreen", gUIManager.title.button);
                    graphicsSettingsRect.y += 100 * y;
                    graphicsSettingsRect.width -= 800 * y;

                    isFullScreen = GUI.Toggle(graphicsSettingsRect, isFullScreen, "", gUIManager.title.button);

                    graphicsSettingsRect.width += 800 * y;
                    graphicsSettingsRect.y -= 100 * y;
                    break;

                case 3:
                    GUI.Box(graphicsSettingsRect, "Vsync", gUIManager.title.button);

                    graphicsSettingsRect.y += 100 * y;
                    graphicsSettingsRect.width -= 800 * y;
                    vSync = GUI.Toggle(graphicsSettingsRect, vSync, "", gUIManager.title.button);

                    graphicsSettingsRect.width += 800 * y;
                    graphicsSettingsRect.y -= 100 * y;
                    break;
                case 4:
                    graphicsSettingsRect.width = 150 * y;
                    graphicsSettingsRect.x += -50 * y;
                    GUI.Box(graphicsSettingsRect, "Back", gUIManager.title.button);
                    if (graphicsSettingsRect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
                    {
                        for (int h = 0; h < Screen.resolutions.Length; h++)
                        {
                            if (Screen.resolutions[h].height == Screen.height)
                            {
                                j = (byte)h;
                                break;
                            }
                        }
                        isFullScreen = Screen.fullScreen;

                        if (QualitySettings.vSyncCount == 1)
                        {
                            vSync = true;
                        }
                        else
                            vSync = false;
                        k = 1;
                    }


                    graphicsSettingsRect.x += 200 * y;
                    GUI.Box(graphicsSettingsRect, "Apply", gUIManager.title.button);
                    if (graphicsSettingsRect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown))
                    {
                        Screen.SetResolution(resolutions[j].width, resolutions[j].height, isFullScreen);

                        if (vSync)
                            QualitySettings.vSyncCount = 1;
                        else
                            QualitySettings.vSyncCount = 0;


                    }
                    break;
            }
        }
    }

    void FindPlayers()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerNetworkManager");
        int i;

        for (i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerNetworkManager>().hasAuthority == true)
            {
                break;
            }
        }

        myPlayer = players[i];
    }
}
