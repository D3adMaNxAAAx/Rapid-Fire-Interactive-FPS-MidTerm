using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpawnSetter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        gameManager.instance.setEnemyCount(0);
    }
    void Start()
    {
        if (gameManager.instance != null)
        {
            gameManager.instance.setPlayerSpawnPos(this.gameObject);
            playerMovement.player.spawnPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
