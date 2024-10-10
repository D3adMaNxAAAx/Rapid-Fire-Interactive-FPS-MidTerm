using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealStats : ScriptableObject
{
    public string itemName;
    public int healAmount;
    public AudioClip healSound;
    public float healCoolDown;
}
