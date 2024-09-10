using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{

    //Allows Designer Communication to Model's Renderer
    [SerializeField] Renderer model;

    //Allows Designer Communication To NavMeshAgent Component 
    [SerializeField] NavMeshAgent agent;

    //Enemy Head Position Tracker(Line Of Sight) For Designer
    [SerializeField] Transform headPos;

    //Enemy SHoot Point Origin Tracker For Designer
    [SerializeField] Transform shootPos;

    //Health Points Tracker and Modifier Field For Designer
    [SerializeField] int HP;

    //Bullet Object Tracker and Communication Field For Designer
    [SerializeField] GameObject bullet;

    //Enemy Fire Rate Modifier Field For Designer
    [SerializeField] float shootRate;

    //Allows Designer To Set Damage Flash Timer 
    [SerializeField] float damageFlashTimer;

    //Enemy Model Original Color Private Tracker
    Color colorOrig;

    //Private Tracker For If Enemy Is Shooting 
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        //Sets our Models original color on scene start
        colorOrig = model.material.color;

        //Tell gameManager To update game goal that an enemy has been added to gameGoal
        //
        //TODO:  Delete this line and uncomment below line on gameManager completion
        //gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Telling our ai to go to the location of the Player's position until game ends or enemy destroyed

        //TODO:  Delete this line and uncomment below line on gameManager completion
        //agent.SetDestination(gameManager.GetInstance().GetPlayer().transform.position);

        //Checking if Enemy is shooting 
        if (!isShooting)
        {
            //Tell enemy to start shooting
            StartCoroutine(shoot());
        }
    }

    IEnumerator shoot()
    {
        //Set shooting to true
        isShooting = true;

        //Create our bullet and fire from the shootPos of Enemy 
        Instantiate(bullet, shootPos.position, transform.rotation);

        //Timer setting shootRate time
        yield return new WaitForSeconds(shootRate);

        //Set shooting back to false
        isShooting = false;

    }

    //Calling our takeDamage method from interface class IDamage
    public void takeDamage(int _amount)
    {
        //Deduct HP on damage recieved
        HP -= _amount;

        //Flash Enemy Red To Indicate Damage Taken
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //Tells Game manager to take 1 enemy out of game goal enemy total

            //TODO:  Delete this line and uncomment below line on gameManager completion
            //gameManager.instance.updateGameGoal(-1);

            //Since No HP Delete Enemy Object
            Destroy(gameObject);
        }
    }

    //Will flash Enmey mesh on damage taken
    IEnumerator flashColor()
    {
        model.material.color = Color.red;

        //Allows Designer to set flash timer 
        yield return new WaitForSeconds(damageFlashTimer);

        //Sets color back to original after timer elapse
        model.material.color = colorOrig;

    }
}
