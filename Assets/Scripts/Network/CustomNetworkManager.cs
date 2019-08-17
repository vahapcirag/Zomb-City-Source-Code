using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{

    [SerializeField] CustomNetworkDiscovery networkDiscovery;

    public string hostAdress;

    public string gameName;

    [SerializeField] bool a = false;

    public List<string> gameAdresses = new List<string>();

    private void Start()
    {
        networkDiscovery = GetComponent<CustomNetworkDiscovery>();
    }
    public void StartAsHost(bool a)
    {
        if(a)
        networkDiscovery.StopBroadcast();
        NetworkServer.Reset();
        
        SetPort();
        singleton.StartHost();
        networkDiscovery.Initialize();
        networkDiscovery.StartAsServer();
    }
    public void ShutDownServer()
    {
        NetworkServer.Shutdown();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
    }


    public void Disconnect()
    {
        singleton.client.Shutdown();
        foreach (GameObject o in FindObjectsOfType<GameObject>())
        {
            Destroy(o);
        }
            
       
        SceneManager.LoadScene(0);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        GameObject player;

        player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            ClientScene.AddPlayer(conn, 1);
    }

    public void SetPort()
    {
        singleton.networkPort = 7777;
    }


    public void SetIpAdress(string i)
    {
        hostAdress = i;
    }


    public void FindGame()
    {
        
        
        networkDiscovery.Initialize();
        networkDiscovery.StartAsClient();
        SetPort();
        StartCoroutine(WaitALittle());
    }

    public void JoinGame()
    {
        SetPort();

        NetworkManager.singleton.networkAddress = hostAdress;
        NetworkManager.singleton.StartClient();
    }

    IEnumerator WaitALittle()
    {
        yield return new WaitForSecondsRealtime(2f);
        gameAdresses = networkDiscovery.GetInformation();
        StopAllCoroutines();
    }


}
