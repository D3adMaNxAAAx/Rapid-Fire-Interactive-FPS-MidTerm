using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    [SerializeField] private int sens;
    [SerializeField] private int lockVertMin, lockVertMax;
    [SerializeField] private bool invertY;

    //zoom settings
    [SerializeField] private float normalFOV = 70f;
    [SerializeField] private float aimingFOV = 40f;
    [SerializeField] private float zoomSpeed = 60f;

    //leaning settings
    [SerializeField] private float leanAngel = 45f;
    [SerializeField] private float leanSpeed = 10f;
    private float currentLeanAngel = 0f;


    private bool isAiming;
    private Camera cam;
    private float rotX;

    public int GetSens()
    {
        return sens;
    }
    public void SetSens(int value)
    {
        sens = value;
    }
    public int GetLockMin()
    {
        return lockVertMin;
    }
    public void SetVertMin(int value)
    {
        lockVertMin = Mathf.Clamp(value, -90, 90);
    }
    public int GetVertMax()
    {
        return lockVertMax;
    }
    public void SetVetMax(int value)
    {
        lockVertMax = Mathf.Clamp(value, -90, 90);
    }
    public float GetLeanAngel()
    {
        return leanAngel;
    }
    public void SetLeanAngel(float value)
    {
        leanAngel = Mathf.Clamp(value, 0, 90);
    }
    public float GetLeanSpeed()
    {
        return leanSpeed;
    }
    public void SetLeanSpeed(float value)
    {
        leanSpeed = Mathf.Max(value, 0);  
    }
    public bool GetInvertY()
    {
        return invertY;
    }
    public void SetInvertY(bool value)
    {
        invertY = value;
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        cam.fieldOfView = normalFOV;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (!invertY)
            rotX -= mouseY;
        else 
            rotX += mouseY;

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);

        HandelLeaning();

        //AimLogic
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, aimingFOV, zoomSpeed * Time.deltaTime);

        }
        else
        {
            isAiming = false;
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, normalFOV, zoomSpeed * Time.deltaTime);

        }
    }

    void HandelLeaning()
    {
        float targetLeanAngel = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            targetLeanAngel = leanAngle;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetLeanAngel = -leanAngle;
        }

        currentLeanAngel = Mathf.LerpAngle(currentLeanAngel, targetLeanAngel, Time.deltaTime * leanSpeed);

        transform.localRotation = Quaternion.Euler(rotX, transform.localEulerAngles.y, currentLeanAngel);
    }
}
