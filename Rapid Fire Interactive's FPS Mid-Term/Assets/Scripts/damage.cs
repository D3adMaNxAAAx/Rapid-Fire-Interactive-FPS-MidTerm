using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class damage : MonoBehaviour {

    //Allows switch between damage types
    [SerializeField] enum damageType { ranged, missle, melee, hazard } // fire is hazard
    [SerializeField] damageType type;

    [SerializeField] ObjectType projectileType; // for object pooling / recycling, ObjectType is enum in projectilePool script
    projectilePool objectPool; // for object pooling / recycling

    public ObjectType getProjectileType() {
        return projectileType;
    }

    //RigidBody component Field tracker, Allows to add velocity to range attacks
    [SerializeField] Rigidbody rb;

    [SerializeField] Transform missleTarget;
    [SerializeField] float missleRotationSpeed = 10; // how much the missle will turn to follow player

    [SerializeField] float damageAmount;
    [SerializeField] float attackSpeed;
    [SerializeField] int destroyTime; //Object destroy timer

    public float getAttackSpeed() {
        return attackSpeed;
    }
    public int getDTime() {
        return destroyTime;
    }

    // Start is called before the first frame update
    void Start() {
        objectPool = FindObjectOfType<projectilePool>();
        if (type == damageType.ranged) {
            // Give velocity to ranged attack 
            rb.velocity = transform.forward * attackSpeed;

            // object is being added to pool and turned off (see playerMovement shoot method)
        }

        if (type == damageType.missle) { // enemySeaking projectile
            missleTarget = gameManager.instance.getPlayer().transform; // getting player position

            // object is being added to pool and turned off (see playerMovement shoot method)
        }
    }

    void Update() {
        if (type == damageType.missle) {
            Vector3 direction = missleTarget.position - rb.position;
            Vector3 rotation = Vector3.Cross(transform.forward, direction);
            rb.angularVelocity = (Vector3.Cross(transform.forward, missleTarget.position - rb.position)) * missleRotationSpeed; // changing where missle is going as the target moves
            rb.velocity = transform.forward * attackSpeed; // keeping bullet speed constant
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore if it collides with another trigger or is tagged as enemy
        if (other.isTrigger || other.CompareTag("Light") || other.CompareTag("Heavy") || other.CompareTag("Basic") || other.CompareTag("Boss") || other.CompareTag("Challenge"))
            return;

        // Object tracker and checker to see if object takes damage
        IDamage dmgObject = other.GetComponent<IDamage>();

        // if it is an object that takes damage we apply damage
        if (dmgObject != null)
        {
            // if object can take damage apply damage amount 
            dmgObject.takeDamage(damageAmount);

        }

        // if it isnt an object that takes damage and damageType was ranged, destroy damage inflicting object
        if (type == damageType.ranged || type == damageType.missle) {

            objectPool.addToPool(projectileType, this.gameObject); // object is not being destory, instead being turned off and added to correct object pool
        }
    }

    private void OnTriggerStay(Collider otherObject) {
        if (otherObject.isTrigger) {
            return;
        }
        if (type == damageType.hazard) { // dealing damage while still in area of effect
            IDamage toDamage = otherObject.GetComponent<IDamage>();
            if (otherObject.CompareTag("Player")) { // only damages player
                toDamage.takeDamage(damageAmount);
            }
        }
    }
}
