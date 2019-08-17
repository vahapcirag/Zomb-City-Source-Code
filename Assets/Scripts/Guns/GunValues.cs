using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GunValues : MonoBehaviour
{
    public Gun specs;
    public float time =0;
    private void Start()
    {
        Invoke("Name", 0.1f);
    }

    private void Update()
    {
        if (time<8)
        {
            time += Time.deltaTime;
        }
    }
    private void Name()
    {
        string path="";

        for (int i = 0; i < gameObject.name.Length; i++)
        {
            if (gameObject.name[i]=='(')
            {
                path = gameObject.name.Remove(i);
                break;
            }
        }

        if (gameObject.name[0] == 'B')
        {
            path = "Baretta";

             specs = Resources.Load<Gun>("ScriptableObjects/Guns/" + path);
        }
        path = "Baretta";
        
       // specs = Resources.Load<Gun>("ScriptableObjects/Guns/" + path);
    }
    
}
