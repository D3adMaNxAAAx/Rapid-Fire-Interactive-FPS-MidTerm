using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    // -- Gun Attributes --
    public GameObject gunModel;
    public float damage;
    public float fireRate;
    public float bulletDist;
    public int ammoCur, ammoMax;
    public bool isSniper;
    public bool isAutomatic;

    // -- Gun Visuals --
    public ParticleSystem hitEffects;
    public AudioClip[] shootSound;
    public float audioVolume;
}
