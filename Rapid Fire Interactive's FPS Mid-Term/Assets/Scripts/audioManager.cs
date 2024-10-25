using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    public static audioManager instance; // Singleton
    public AudioMixerGroup SFXMixerGroup;
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
    [SerializeField] public AudioClip audHeal; 

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

    [SerializeField] public AudioClip fartEgg;

    void Start()
    {
        instance = this;
    }
    public void PlayFootstep()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup; // Set the audio source to use the SFX mixer group
        audioSource.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepVol);  // Play random footstep
    }

    public void PlayJump()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup; // Set the audio source to use the SFX mixer group
        audioSource.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);  // Play random jump sound
    }

    public void PlayHurt()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup; // Set the audio source to use the SFX mixer group
        audioSource.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);  // Play random hurt sound
    }

    public void PlayButtonHover()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup;
        audioSource.PlayOneShot(audButtonHover, audButtonHoverVol);
    }

    public void PlayButtonClick()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup;
        audioSource.PlayOneShot(audButtonClick, audButtonClickVol);
    }
    public void PlayHeadshot()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixerGroup;
        audioSource.PlayOneShot(headShotA, 1.0f); // Adjust volume as needed
    }
}
