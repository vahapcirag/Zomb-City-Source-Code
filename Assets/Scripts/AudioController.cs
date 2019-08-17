using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController : NetworkBehaviour
{
    public AudioSource audioSource;

    
    [ClientRpc]
    public void RpcMakeSomeNoise(string a)
    {
        audioSource.clip = Resources.Load<AudioClip>("Audios/ZombieVolumeEffects/"+a);
        audioSource.Play();
    }
}
