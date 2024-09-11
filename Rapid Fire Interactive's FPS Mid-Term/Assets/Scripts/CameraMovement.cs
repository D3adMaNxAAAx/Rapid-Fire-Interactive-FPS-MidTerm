using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
  
    public float normalFOV = 70f;
    public float aimingFOV = 40f;
    public float zoomSpeed = 60f;

    public float leanAngle = 45f;
    public float leanSpeed = 10f;
    private float currentLeanAngle = 0f;

    private bool isAiming;
    private Camera cam;

    float rotX;

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
    public float GetLeanAngle()
    {
        return leanAngle;
    }
    public void SetLeanAngel(float value)
    {
        leanAngle = Mathf.Clamp(value, 0, 90);
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
            rotX += mouseX;

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
        float targetLeanAngle = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            targetLeanAngle = leanAngle;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetLeanAngle = -leanAngle;
        }

        currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, Time.deltaTime * leanSpeed);

        transform.localRotation = Quaternion.Euler(rotX, transform.localEulerAngles.y, currentLeanAngle);
    }
}
