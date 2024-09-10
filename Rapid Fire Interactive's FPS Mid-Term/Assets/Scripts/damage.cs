using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{


    //Allows switch between damage types
    [SerializeField] enum damageType { ranged, stationary }

    //Modifier field for damage type for designer
    [SerializeField] damageType type;

    //RigidBody component Field tracker for designer 
    //Allows to add velocity to range attacks
    [SerializeField]Rigidbody rb;

    //Modifier for damageAmount by object for designer
    [SerializeField] int damageAmount;

    //Modifier Field for attack speed for designer
    [SerializeField] int attackSpeed;

    //Object destroy timer field for designer
    [SerializeField] int destroyTime;
    // Start is called before the first frame update



    void Start()
    {
        //Check if ranged attack and give velocity
        if (type == damageType.ranged)
        {
            //Give velocity to ranged attack 
            rb.velocity = transform.forward * attackSpeed;

            //after so long of no hit we delete range attack object
            Destroy(gameObject, destroyTime);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //Object tracker and checker to see if object takes damage
        IDamage dmgObject = other.GetComponent<IDamage>();

        //if it is an object that takes damage we apply damage
        if (dmgObject != null)
        {
            //if object can take damage apply damage amount 
            dmgObject.takeDamage(damageAmount);

        }

        //if it isnt an object that takes damage and damageType was ranged destroy damage inflicting object
        if (type == damageType.ranged)
        {
            Destroy(gameObject);
        }
    }
}
