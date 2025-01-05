using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerSpawnSetter : MonoBehaviour {

    [SerializeField] bool boss = false;

    // Start is called before the first frame update
    private void Awake() {
        gameManager.instance.setEnemyCount(0);
    }

    void Start() {
        if (gameManager.instance != null) {
            setInitialSpawn();
        }
    }

    void Update() {
        if (boss) {
            if (gameManager.instance.getPauseStatus() == true) { // for if player spawns out of bounds in boss level, this should hopefully be a work around
                if (Input.GetKey("s") && Input.GetKey("p") && Input.GetKey("a") && Input.GetKey("w") && Input.GetKey("n")) { 
                    setInitialSpawn();
                }
            }
        }
    }

    public void setInitialSpawn() {
        gameManager.instance.setPlayerSpawnPos(this.gameObject);
        playerMovement.player.spawnPlayer();
    }

}
