using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu]
public class objectives : ScriptableObject
{
    public enum objectiveType { goHere, collectThis, killThese};
    
    public string objective;

    public bool isCompleted;
    
}
