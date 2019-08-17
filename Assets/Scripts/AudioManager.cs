using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audio_ = new AudioClip[1];
    public AudioSource audioSource;

    bool check = false;
    void Start()
    {
        audioSource.clip = audio_[0];
        audioSource.Play();
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {

        if ((UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 2))
        {
            audioSource.clip = audio_[1];
            audioSource.Play();

        }
    }


   
 
}
