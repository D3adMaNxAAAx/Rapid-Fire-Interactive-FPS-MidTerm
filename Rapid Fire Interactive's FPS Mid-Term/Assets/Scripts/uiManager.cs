using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiManager : MonoBehaviour
{
    public static uiManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        if (manager == null){
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
