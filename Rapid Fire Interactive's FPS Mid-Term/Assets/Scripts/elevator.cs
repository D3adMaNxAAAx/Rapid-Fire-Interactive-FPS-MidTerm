using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class elevator : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject elevCallButton;
    [SerializeField] GameObject elevNextLevel;
    [SerializeField] GameObject elevDoor;
    [SerializeField] Canvas obj1; //These are for marking our objectives in game to inform the player on why the door doesn't open.
    [SerializeField] Canvas obj2;
    //[Range(0,20)][SerializeField] int requiredPower = 0;
    [SerializeField] bool moveToScene;
    [SerializeField] bool killObjective;
    [SerializeField] loadingScreen screen;

    int requiredPower = 3;
    int sceneNum;
    int powerAmount;
    Vector3 doorPos;
    Vector3 doorMovePos;

    Canvas objStorage;

    bool uIIsPopping;

    bool isDoor;
    bool movingScene;
    // Start is called before the first frame update
    void Start()
    {
        if (obj1 != null)
            obj1.enabled = false;
        if (obj2 != null)
            obj2.enabled = false;
        if (moveToScene)
            sceneNum = SceneManager.GetActiveScene().buildIndex + 1;
        if (elevDoor != null)
        {
            doorPos = elevDoor.transform.position;
            doorMovePos = new Vector3(doorPos.x, doorPos.y, doorPos.z + 3.5f);
        }
        if (screen == null)
        {
            screen = GameObject.Find("Loading Screen Obj").GetComponent<loadingScreen>();
        }
        if (gameManager.instance.getInteractUI().activeInHierarchy)
        {
            gameManager.instance.getInteractUI().SetActive(false);
        }
    }

    private void Update()
    {
        if (elevDoor != null)
        { 
            if (isDoor)
            {
                StartCoroutine(openDoor());
            }
            else
            {
                StartCoroutine(closeDoor());
            }
        }
        setPower(playerStats.Stats.getPWRLevel());
    }
    public void setPower(int amount)
    {
        powerAmount = amount;
    }

    public void interact()
    {
        if (elevNextLevel != null)
        {
            elevCallButton.GetComponent<elevator>().disableDoorBool();
            StartCoroutine(nextScene());
        }
        else if (elevCallButton != null)
        {
            if (!killObjective)
            {
                if (powerAmount >= requiredPower && playerStats.Stats.getBadgesFound() >= 9) // this needs to be set to whatever the amount of power we need
                {
                    isDoor = true;
                }
                else if (powerAmount < 3)
                {
                    if (!uIIsPopping && obj1 != null)
                    {
                        objStorage = obj1;
                        if (!uIIsPopping)
                            StartCoroutine(popUIStuff());
                    }
                }
                else
                {
                    if (!uIIsPopping && obj2 != null)
                    {
                        objStorage = obj2;
                        if (!uIIsPopping)
                            StartCoroutine(popUIStuff());
                    }
                }
            }
            else
            {
                if (gameManager.instance.getEnemyCount() <= 0)
                {
                    isDoor = true;
                }
                else
                {
                    objStorage = obj1;
                    if (!uIIsPopping)
                        StartCoroutine(popUIStuff());
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!gameManager.instance.getInteractUI().activeInHierarchy)
                gameManager.instance.getInteractUI().SetActive(true);
            if (Input.GetButton("Interact"))
            {
                interact();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        gameManager.instance.getInteractUI().SetActive(false);
    }

    IEnumerator openDoor()
    {
        float a = Time.deltaTime * 3f;
        //Use of Lerp to get smooth door movement
        elevDoor.transform.position = Vector3.Lerp(elevDoor.transform.position, doorMovePos, a);
        yield return null;
    }
    IEnumerator closeDoor()
    {
        float a = Time.deltaTime * 3;
        //Use of Lerp to get smooth door movement
        elevDoor.transform.position = Vector3.Lerp(elevDoor.transform.position, doorPos, a);
        yield return null;
    }
    IEnumerator nextScene()
    {
        if (!movingScene && moveToScene)
        {
            movingScene = true;
            yield return new WaitForSeconds(3f);
            screen.loadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    IEnumerator popUIStuff()
    {
        uIIsPopping = true;
        objStorage.enabled = true;
        yield return new WaitForSeconds(1.2f);
        objStorage.enabled = false;
        objStorage = null;
        uIIsPopping = false;
    }
    public void disableDoorBool()
    {
        isDoor = false;
    }
}
