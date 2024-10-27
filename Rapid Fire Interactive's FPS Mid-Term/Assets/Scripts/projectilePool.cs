using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]

public enum ObjectType { ARShot, HandCannonShot, LaserShot, PistolShot, ShottyShot, SniperShot, SMGShot, 
BossShot, DreadSpit, FatDreadSpit, FatSeeker, MassiveDreadShot, Seeker } // these are set in damage script on bullets in unity

[System.Serializable]

public class ObjectPool { // first class

    public ObjectType objectType;
    public GameObject projectile;
    public Queue<GameObject> objectQueue; // objects are stored and grabbed from here

}

public class projectilePool : MonoBehaviour { // second class

    public static projectilePool thePool; // singleton
    public ObjectPool[] objectPoolsArray; // array of each object's pool, projectiles are manually added (dragged into inspector) in unity (13 of them)

    void Awake() {
        if (thePool == null) {
            DontDestroyOnLoad(gameObject); // this script will stay between scenes
            thePool = this;
        }
        else if (thePool != null) {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        foreach (ObjectPool tempObjectPool in objectPoolsArray) { // 13
            tempObjectPool.objectQueue = new Queue<GameObject>();
        }
    }

    public void addToPool(ObjectType projectileType, GameObject usedProjectile) {
        findProjectilePool(projectileType).objectQueue.Enqueue(usedProjectile); // add to pool
        usedProjectile.SetActive(false); // hide object while in pool
    }

    public GameObject getProjectileFromPool(ObjectType projectileType) {
        ObjectPool currentPool = findProjectilePool(projectileType); // current pool now is the pool that holds the searched for type of projectile
        if (currentPool.objectQueue.Count > 0) { // if there is an object in the pool, use it
            return currentPool.objectQueue.Dequeue();
        }
        else { // if there is not an object in the pool, create a new object to be used
            return Instantiate(currentPool.projectile);
        }
    }

    public ObjectPool findProjectilePool(ObjectType searchType) {
        ObjectPool foundObject = null;
        foreach(ObjectPool tempObjectPool in objectPoolsArray) {
            if (tempObjectPool.objectType == searchType) { 
                foundObject = tempObjectPool;
                return foundObject; // pool type that matches the searched projectile type
            }
        }
        // else
        return null;
    }
}
