using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetOnLoad : MonoBehaviour
{
    List<GameObject> pwrIs;
    List<GameObject> idCards;
    bool isDone;
    // Start is called before the first frame update
    private void Update()
    {
        if (!isDone)
        {
            isDone = true;
            pwrIs = holdMe.instance.getPwrItems();
            idCards = holdMe.instance.getIDImages();
            if (playerStats.Stats != null)
            {
                playerStats.Stats.resetOBJStats();
                for (int i = 0; i < pwrIs.Count; i++)
                {
                    pwrIs[i].SetActive(false);
                }
                for (int i = 0; i < idCards.Count; i++)
                {
                    idCards[i].SetActive(false);
                }
            }
        }
    }
}
