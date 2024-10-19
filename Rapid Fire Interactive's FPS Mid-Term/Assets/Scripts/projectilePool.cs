using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum ObjectType { aRShot, stopIt } // these are set in damage

[System.Serializable]

public class ObjectPool { // first class

    public ObjectType objectType;
    public GameObject projectile;
    public Queue<GameObject> objectQueue; // objects are stored and grabbed from here

}

public class projectilePool : MonoBehaviour { // second class

    public ObjectPool[] objectPoolsArray; // array of each object's pool, projectiles are manually added (dragged into inspector) in unity (13 of them)

    void Start() {
        foreach (ObjectPool tempObjectPool in objectPoolsArray) { // 13
            tempObjectPool.objectQueue = new Queue<GameObject>();
        }
    }

    public void addToPool(ObjectType projectileType, GameObject usedProjectile) {
        usedProjectile.SetActive(false);
        findProjectilePool(projectileType).objectQueue.Enqueue(usedProjectile);
    }

    public GameObject getProjectileFromPool(ObjectType projectileType) {
        ObjectPool currentPool = findProjectilePool(projectileType);
        if (currentPool.objectQueue.Count > 0) { // if there is an object in the pool, use it
            return currentPool.objectQueue.Dequeue();
        }
        else { // if there is not an object in the pool, create a new object to be used
            return Instantiate(currentPool.projectile);
        }
    }

    private ObjectPool findProjectilePool(ObjectType searchType) {
        ObjectPool foundObject = null;
        foreach(ObjectPool tempObjectPool in objectPoolsArray) {
            if (tempObjectPool.objectType == searchType) { 
                foundObject = tempObjectPool;
            }
        }
        return foundObject;
    }
}
