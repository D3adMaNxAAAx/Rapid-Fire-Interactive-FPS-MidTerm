using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class elevator : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject elevCallButton;
    [SerializeField] GameObject elevNextLevel;
    [SerializeField] GameObject elevDoor;

    int powerAmount;
    Vector3 doorPos;
    Vector3 doorMovePos;

    bool isDoor;
    bool movingScene;
    // Start is called before the first frame update
    void Start()
    {
        if (elevDoor != null)
        {
            doorPos = elevDoor.transform.position;
            doorMovePos = new Vector3(doorPos.x, doorPos.y, doorPos.z + 3.5f);
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
            if (powerAmount >= 0) // this needs to be set to whatever the amount of power we need
            {
                isDoor = true;
            }
            else
            {
                Debug.Log("Not Enough Power");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetButton("Interact"))
            {
                interact();
            }
        }
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
        Debug.Log("closing door");
        float a = Time.deltaTime * 3;
        //Use of Lerp to get smooth door movement
        elevDoor.transform.position = Vector3.Lerp(elevDoor.transform.position, doorPos, a);
        yield return null;
    }
    public void disableDoorBool()
    {
        isDoor = false;
    }
    IEnumerator nextScene()
    {
        if (!movingScene)
        {
            movingScene = true;
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }
}
