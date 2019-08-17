using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]public GUISkin title;
    [SerializeField]public GUISkin main;

    int screenWidth, screenHeight;

    public float x, y; //The x and y is ratio

    

    private void OnLevelWasLoaded(int level)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            if (i == level)
            {
                for (int j = 0; j < transform.childCount; j++)
                {
                    if (transform.GetChild(j).GetSiblingIndex() == level)
                    {
                        transform.GetChild(j).gameObject.SetActive(true);
                    }

                    else
                        transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0))
        {
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        }

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        x = (float)screenWidth / 1920;
        y = (float)screenHeight / 1080;

        title.box.fontSize = main.box.fontSize;
        title.box.fontSize = (int)(title.box.fontSize * x);

        title.button.fontSize = main.button.fontSize;
        title.button.fontSize = (int)(title.button.fontSize * x);

        title.textField.fontSize = main.textField.fontSize;
        title.textField.fontSize = (int)(title.textField.fontSize * x);
        DontDestroyOnLoad(gameObject);
    }

   
}
