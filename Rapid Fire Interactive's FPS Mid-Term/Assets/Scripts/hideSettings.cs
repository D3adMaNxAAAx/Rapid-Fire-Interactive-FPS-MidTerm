using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hideSettings : MonoBehaviour
{
    void Awake()
    {
        if (keepSettings.gameSettings == null)
        {
            this.gameObject.SetActive(false);
        }
    }
}
