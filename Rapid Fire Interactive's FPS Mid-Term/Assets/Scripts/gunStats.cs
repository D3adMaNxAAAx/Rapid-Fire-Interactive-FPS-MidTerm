using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject {
    // -- Gun Attributes --
    public GameObject gunModel;
    public GameObject projectile;
    public ObjectType weaponName;
    public enum ObjectType { AR, Pistol, SMG, Shotgun, Sniper, LaserRifle, HandCannon } // DON"T select anything on this one in Unity
    public float damage;
    public float fireRate;
    public float reloadTime;
    public int range;
    public int ammoCur, ammoMag, ammoMax, ammoOrig;
    public bool isAutomatic;

    // -- Gun Visuals --
    public ParticleSystem hitEffects;
    public AudioClip[] shootSound;
    public float audioVolume;
    public gunColor color;
    public enum gunColor { red = 1, blue = 2, green = 3, orange = 4 }

    // -- Gun Image --
    public Sprite gunIcon;
}
