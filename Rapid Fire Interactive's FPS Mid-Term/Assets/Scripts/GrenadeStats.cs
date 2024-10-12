using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GrenadeStats : ScriptableObject
{
    public GameObject grenadeModel;
    public float explosionDelay;
    public float explosionRadius;
    public int explosionDamage; // grenade does double damage for some reason
    public ParticleSystem explosionEffect;
    public AudioClip explosionSound;
    public AudioClip pinSound;
    public float throwForce;
    public bool isSticky;
}
