using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    Color colorOrig;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && transform.position != gameManager.instance.getPlayerSpawnPos().transform.position)
        {
            gameManager.instance.setPlayerSpawnPos(this.gameObject); //setting spwan point
            StartCoroutine(flashColor());
        }
    }
    IEnumerator flashColor()
    {
        model.material.color = Color.blue;
        gameManager.instance.getCheckPointPopup().SetActive(true); //Activate popup
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.getCheckPointPopup().SetActive(false);  //Deactivate Popup
        model.material.color = colorOrig;
    }
}
