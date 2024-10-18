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
    public int ammoCur, ammoMag, ammoMax, ammoOrig;
    public bool isSniper;
    public bool isAutomatic;
    public bool isLaser;
    public bool isShotgun;

    // -- Gun Visuals --
    public ParticleSystem hitEffects;
    public AudioClip[] shootSound;
    public float audioVolume;
    public gunColor color;
    public enum gunColor { red = 1, blue = 2, green = 3, orange = 4 }

    // -- Gun Image --
    public Sprite gunIcon;
}
