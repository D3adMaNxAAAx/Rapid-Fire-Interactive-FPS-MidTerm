using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance; // Singleton
    // -- Audio Clips --
    [Header("-- Game Music --")]
    [SerializeField] public AudioClip menuMusic;
    [SerializeField] public AudioClip levelMusic;
    [SerializeField] public AudioClip bossMusic;

    [Header("-- Player Sounds --")]
    [SerializeField] public AudioClip[] audSteps;
    [SerializeField] public AudioClip[] audJump;
    [SerializeField] public AudioClip[] audHurt;
    [SerializeField] public AudioClip headShotA;

    [Header("-- Enemy Sounds --")]
    [SerializeField] public AudioClip[] audEnemySteps;

    [Header("-- Player Volume --")]
    [Range(0, 1)] [SerializeField] public float audStepVol;
    [Range(0, 1)] [SerializeField] public float audJumpVol;
    [Range(0, 1)] [SerializeField] public float audHurtVol;

    [Header("-- Game Sounds --")]
    [SerializeField] public AudioClip VictoryA;
    [SerializeField] public AudioClip audHeal; // I think there is already an audio in playerMovement for this?

    [Header("-- UI Sounds --")]
    [SerializeField] public AudioClip audButtonHover;
    [SerializeField] public AudioClip audButtonClick;

    // -- Volumes --

    [Header("-- Game Music Volume --")]
    [Range(0, 1)] [SerializeField] public float menuMusicVol;
    [Range(0, 1)] [SerializeField] public float levelMusicVol;
    [Range(0, 1)] [SerializeField] public float bossMusicVol;

    [Header("-- Enemy Volume --")]
    [Range(0, 1)] [SerializeField] public float audEnemyStepVol;

    [Header("-- Game Volume --")]
    [Range(0, 1)] [SerializeField] public float audHealVol;

    [Header("-- UI Volume --")]
    [Range(0, 1)] [SerializeField] public float audButtonHoverVol;
    [Range(0, 1)] [SerializeField] public float audButtonClickVol;

    void Start()
    {
        instance = this;
    }
}
