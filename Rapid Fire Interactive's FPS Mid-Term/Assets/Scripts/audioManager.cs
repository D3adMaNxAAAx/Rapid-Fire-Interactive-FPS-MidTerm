using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance; // Singleton

    [Header("-- Player Sounds --")]
    [SerializeField] public AudioClip[] audSteps;
    [SerializeField] public AudioClip[] audJump;
    [SerializeField] public AudioClip[] audHurt;

    [Header("-- Player Volume --")]
    [Range(0, 1)] [SerializeField] public float audStepVol;
    [Range(0, 1)][SerializeField] public float audJumpVol;
    [Range(0, 1)][SerializeField] public float audHurtVol;

    [Header("-- Enemy Sounds --")]
    [SerializeField] public AudioClip[] audEnemySteps;

    [Header("-- Enemy Volume --")]
    [Range(0, 1)] [SerializeField] public float audEnemyStepVol;

    [Header("-- Game Sounds --")]
    [SerializeField] public AudioClip audHeal;

    [Header("-- Game Volume --")]
    [Range(0, 1)] [SerializeField] public float audHealVol;

    [Header("-- UI Sounds --")]
    [SerializeField] public AudioClip audButtonHover;
    [SerializeField] public AudioClip audButtonClick;

    [Header("-- UI Volume --")]
    [Range(0, 1)][SerializeField] public float audButtonHoverVol;
    [Range(0, 1)] [SerializeField] public float audButtonClickVol;

    void Start()
    {
        instance = this;
    }
}
