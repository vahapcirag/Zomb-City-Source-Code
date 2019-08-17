using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class LobbyManager : NetworkBehaviour
{
    CustomNetworkManager networkManager;

    [SerializeField] List<PlayerNetworkManager> playerNetworkManagers = new List<PlayerNetworkManager>();
    bool rider;
    bool robotic;



    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {

        for (byte i = 0; i < playerNetworkManagers.Count; i++)
        {
            if (playerNetworkManagers[i].characterSlot_1)
            {
                for (byte j = 0; j < playerNetworkManagers.Count; j++)
                {

                    CmdSetAllTrue(j, 1);

                }
            }
            else if (playerNetworkManagers[i].characterSlot_2)
            {
                for (byte j = 0; j < playerNetworkManagers.Count; j++)
                {

                    CmdSetAllTrue(j, 2);
                }
            }
        }
    }

    public void FindNetworkManagers()
    {
        List<GameObject> gOPlayerNetworkManagers = GameObject.FindGameObjectsWithTag("PlayerNetworkManager").OfType<GameObject>().ToList();
        List<PlayerNetworkManager> list = new List<PlayerNetworkManager>();

        for (int i = 0; i < gOPlayerNetworkManagers.Count; i++)
        {
            list.Add(gOPlayerNetworkManagers[i].GetComponent<PlayerNetworkManager>());
        }

        playerNetworkManagers = list;
    }

    [Command]
    void CmdSetAllTrue(byte i, byte a)
    {
        playerNetworkManagers[i].CmdSetCharacter(a);
    }

}
