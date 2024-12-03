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
    private StatusEffectUIManager uiManager;

    public ObjectType getProjectileType() {
        return projectileType;
    }

    //RigidBody component Field tracker, Allows to add velocity to range attacks
    [SerializeField] Rigidbody rb;

    [SerializeField] Transform missleTarget;
    [SerializeField] float missleRotationSpeed = 10; // how much the missle will turn to follow player

    [SerializeField] float damageAmount;
    [SerializeField] float attackSpeed;
    [SerializeField] float burnEffectDuration = 5f; // Duration for lingering burn
    [SerializeField] float burnEffectDamage = 0.5f; // Low damage for lingering burn

    static Vector3 startingPosition;
    static float distance;
    static int range;

    public float getAttackSpeed() {
        return attackSpeed;
    }

    public void setCurrentPosAndRange(Vector3 pos, int bulletRange) {
        distance = 0;
        startingPosition = pos;
        range = bulletRange;
    }

    // Start is called before the first frame update
    void Start() {
        objectPool = FindObjectOfType<projectilePool>();

        if (type == damageType.ranged) {
            // Give velocity to ranged attack 
            rb.velocity = transform.forward * attackSpeed;

            // object is being added to pool and turned off after is gets to its max range instead of being destroyed
        }

        if (type == damageType.missle) { // enemySeaking projectile
            missleTarget = gameManager.instance.getPlayer().transform; // getting player position

            // object is being added to pool and turned off after is gets to its max range instead of being destroyed
        }
    }

    void Update() {
        if (type == damageType.ranged || type == damageType.missle) {
            distance = Vector3.Distance(startingPosition, transform.position);
            if (distance > range) {
                objectPool.addToPool(projectileType, this.gameObject); // object is not being destory, instead being turned off and added to correct object pool
                distance = 0;
            }
        }
        if (type == damageType.missle) {
            if (playerMovement.player != null)
            {
                Vector3 direction = missleTarget.position - rb.position;
                Vector3 rotation = Vector3.Cross(transform.forward, direction);
                rb.angularVelocity = (Vector3.Cross(transform.forward, missleTarget.position - rb.position)) * missleRotationSpeed; // changing where missle is going as the target moves
                rb.velocity = transform.forward * attackSpeed; // keeping bullet speed constant
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // so enemies don't hurt themselves
        if (other.isTrigger || other.CompareTag("Light") || other.CompareTag("Heavy") || other.CompareTag("Basic") || other.CompareTag("Elder Demon") || other.CompareTag("Challenge")
            || other.CompareTag("Basic Melee") || other.CompareTag("Ranged Heavy") || other.CompareTag("Demon Golem")) {
            return;
        }

        // Object tracker and checker to see if object takes damage
        IDamage dmgObject = other.GetComponent<IDamage>();

        // if it is an object that takes damage we apply damage
        if (dmgObject != null) {
            dmgObject.takeDamage(damageAmount);
            if (type == damageType.melee) { // to make sure player doesn't get hit twice
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            if (projectileType == ObjectType.MassiveDreadShot) { // applying burn to Demon Golem Ranged attacks
                playerMovement.player.ApplyBurningEffect();
            }
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

                uiManager = FindObjectOfType<StatusEffectUIManager>();
                if (uiManager != null) {
                    uiManager.ShowBurningEffect();  // Show burning icon when the effect starts
                }
            }
        }
    }

    private void OnTriggerExit(Collider otherObject)
    {
        if (otherObject.isTrigger)
        {
            return;
        }
        if (type == damageType.hazard)
        { // dealing damage while still in area of effect
            if (otherObject.CompareTag("Player"))
            {
                uiManager = FindObjectOfType<StatusEffectUIManager>();
                if (uiManager != null)
                {
                    uiManager.HideBurningEffect();
                }
                //StartCoroutine(ApplyBurnEffect(otherObject.GetComponent<IDamage>()));
            }
        }
    
    }

    private IEnumerator ApplyBurnEffect(IDamage target)
    {
       
        if (target == null) yield break;
        float elapsed = 0;
        uiManager = FindObjectOfType<StatusEffectUIManager>();

        if (uiManager != null)
        {
            uiManager.ShowBurningEffect(); // Show burn effect in UI
        }
        while (elapsed < burnEffectDuration)
        {
            target.takeDamage(burnEffectDamage);
            elapsed += 1f;
            yield return new WaitForSeconds(1f); // Apply burn damage every second
        }
        if (uiManager != null)
        {
            uiManager.HideBurningEffect(); // Hide burn effect after duration
        }


    }
}
