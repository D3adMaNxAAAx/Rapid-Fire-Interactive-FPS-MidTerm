using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public float fireRate;
    public float bulletDist;
    public int damage;
    public int ammoCur;
    public int ammoMax;
    public ParticleSystem hitEffects;
    public AudioClip[] shootSound;
    public float audioVolume;
    public bool isSniper;
}
