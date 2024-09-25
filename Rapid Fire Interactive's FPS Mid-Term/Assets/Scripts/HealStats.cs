using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealStats : ScriptableObject
{
    public string itemName;
    public int healAmmount;
    public AudioClip healSound;
    public float healCoolDown;
}
