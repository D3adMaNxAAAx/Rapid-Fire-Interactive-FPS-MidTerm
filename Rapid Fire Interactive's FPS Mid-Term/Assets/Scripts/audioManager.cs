using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance; // Singleton

    [Header("-- Player Sounds --")]
    [SerializeField] AudioClip[] playerSteps;

    [Header("-- Player Volume --")]
    [Range(0,1)] [SerializeField] float playerStepVol;

    [Header("-- Enemy Sounds --")]
    [SerializeField] AudioClip[] enemySteps;

    [Header("-- Enemy Volume --")]
    [Range(0, 1)] [SerializeField] float enemyStepVol;

    [Header("-- UI Sounds --")]
    [SerializeField] AudioClip buttonClick;

    [Header("-- UI Volume --")]
    [Range(0, 1)][SerializeField] float buttonClickVol;

    void Start()
    {
        instance = this;
    }
}
