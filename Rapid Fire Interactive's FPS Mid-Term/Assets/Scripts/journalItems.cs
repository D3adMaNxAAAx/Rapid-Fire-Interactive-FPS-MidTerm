using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]

public class journalItems : ScriptableObject
{
    public GameObject objectModel;

    [SerializeField] Sprite objectIcon;
    


    public bool isPower;
    public bool isIdBadge;
    public bool isLostDoc;
  
}
