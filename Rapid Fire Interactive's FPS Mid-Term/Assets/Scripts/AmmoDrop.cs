using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] GameObject ammoDrop;
    [Range(0,100)]
    [SerializeField] int ammoRefillPercent;
    [SerializeField] float rotateSpeed;
    [SerializeField] bool rotateClockwise;

    GameObject player;
    Vector3 rotDirection;

    void Start()
    {
        ammoDrop = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateClockwise) {
            transform.Rotate(0, rotateSpeed, 0 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.getPlayerScript().addAmmo(ammoRefillPercent);
            playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.coinPickupA, audioManager.instance.coinPickupVol);
            Destroy(ammoDrop);
        }
        else
        {
            return;
        }
    }
}
