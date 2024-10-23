using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class mainMenuHandler : MonoBehaviour
{
    public static mainMenuHandler instance;

    private void Start()
    {
        instance = this;
    }

    
    [SerializeField] AudioSource aud;

}
