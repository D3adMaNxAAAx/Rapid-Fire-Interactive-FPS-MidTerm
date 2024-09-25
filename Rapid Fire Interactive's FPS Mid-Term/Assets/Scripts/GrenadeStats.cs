using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GrenadeStats : ScriptableObject
{
    public GameObject grenadeModel;
    public float explosionDelay;
    public float explosionRadius;
    public int explosionDamage;
    public ParticleSystem explosionEffect;
    public AudioClip explosionSound;
    public float throwForce;
    public bool isSticky;
}
