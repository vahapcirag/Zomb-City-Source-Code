using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    public List<string> GetInformation()
    {
        List<string> gameAdresses = new List<string>();

        foreach (KeyValuePair<string,NetworkBroadcastResult> item in broadcastsReceived)
        {
            
            gameAdresses.Add(item.Key.Substring(7));
        }

        
        return gameAdresses;
    }
    
}
