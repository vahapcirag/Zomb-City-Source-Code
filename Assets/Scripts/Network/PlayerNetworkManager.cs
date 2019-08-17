using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerNetworkManager : NetworkBehaviour
{

    [SyncVar] public byte characterIndex;
    public GameObject gameManager;
    [SerializeField] LobbyManager gameManagerComponent;

    [SyncVar] public bool characterSlot_1 = false;
    [SyncVar] public bool characterSlot_2 = false;

    bool check = false;
    CustomNetworkManager networkManager;
    CustomNetworkDiscovery networkDiscovery;

    
    private void Start()
    {
        characterIndex = 0;
        DontDestroyOnLoad(gameObject);

        if (isServer)
        {
            CmdSpawnGameManager();
            InvokeRepeating("FindGameManager", 0.1f, 1f);
        }
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        networkDiscovery = GameObject.Find("NetworkManager").GetComponent<CustomNetworkDiscovery>();

    }

    private void Update()
    {

        if (!check && (SceneManager.GetActiveScene().buildIndex == 2) && isLocalPlayer)
        {
            CancelInvoke();
            ClientScene.Ready(connectionToServer);
            CmdSpawnPlayerUnit();
            check = true;
        }
    }
    private void FindGameManager()
    {
        if (isServer && (GameObject.FindGameObjectWithTag("GameController") != null))
        {
            gameManagerComponent = GameObject.FindGameObjectWithTag("GameController").GetComponent<LobbyManager>();
            gameManagerComponent.FindNetworkManagers();
        }

    }

    [Command]
    public void CmdSpawnGameManager()
    {
        if (GameObject.FindGameObjectWithTag("GameController") != null)
        {
            return;
        }
        GameObject go = Instantiate(gameManager);


        go.transform.position = Vector3.zero;

        NetworkServer.Spawn(go);
    }

    [Command]
    public void CmdSpawnPlayerUnit()
    {
        GameObject go = Instantiate(Character());

        go.transform.position = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    public void CmdChangeIndex(byte i)
    {
        characterIndex = i;
    }

    [Command]
    public void CmdSetCharacter(byte a)
    {
        if (a == 1)
        {
            characterSlot_1 = true;
        }
        else if (a == 2)
        {
            characterSlot_2 = true;
        }
    }

    private GameObject Character()
    {
        GameObject character = new GameObject();

        switch (characterIndex)
        {
            case 1:
                character = Resources.Load<GameObject>("Characters/Prefabs/RiderPrefabs/RiderUnit");
                break;
            case 2:

                character = Resources.Load<GameObject>("Characters/Prefabs/RoboticPrefabs/RoboticUnit");
                break;
        }

        return character;
    }

    [Command]
    public void CmdChangeScene()
    {
        networkManager.ServerChangeScene("TheGame");
    }


    
}
