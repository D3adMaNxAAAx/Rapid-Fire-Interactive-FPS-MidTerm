using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    //Interface IDamage class 
    //Allows any derived object of class to take Damage
    void takeDamage(float amount);

}
