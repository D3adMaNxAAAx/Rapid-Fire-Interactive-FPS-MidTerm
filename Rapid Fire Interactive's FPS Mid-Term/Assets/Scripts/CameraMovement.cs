using System.Collections;
using System.Collections.Generic;

//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement state;
    //Sensitivity settings 
    [SerializeField] private Slider sensitivitySlider;
    
    [SerializeField] private int sens = 300;
    int zoomSens;
    int startingSens;
  
    [SerializeField] private int lockVertMin, lockVertMax;
    [SerializeField] private bool invertY;

    //zoom settings
    [SerializeField] private float normalFOV = 70f;
    [SerializeField] private float aimingFOV = 40f;
    [SerializeField] private float zoomSpeed = 60f;
    [SerializeField] private bool snapZoom = false;

    //leaning settings
    [SerializeField] private float leanAngle = 30f;
    [SerializeField] private float leanSpeed = 10f;
    [SerializeField] private float LeanOffSet = 0.8f;
    private float currentLeanAngle = 0f;
    private Vector3 OrigCameraPos;

   
 

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
    public float GetLeanAngle()
    {
        return leanAngle;
    }
    public void SetLeanAngle(float value)
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

    public void AdjustSensitivity(float value)
    {
        sens = (int)value;
        Debug.Log("Sensitivity adjusted to: " + sens);
    }
   
    // Start is called before the first frame update
    void Start()
    {
        state = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        cam.fieldOfView = normalFOV;
        OrigCameraPos = transform.localPosition;
        startingSens = sens;
        zoomSens = sens / 2;

        sensitivitySlider.value = sens;
        sensitivitySlider.onValueChanged.AddListener(AdjustSensitivity);

    }

    // Update is called once per frame
    void Update()
    {

        sens = (int)sensitivitySlider.value;

        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        if (!invertY)
            rotX -= mouseY;
        else 
            rotX += mouseY;

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);

        HandleLeaning();

        //AimLogic
        if (Input.GetMouseButton(1)) {
            if (gameManager.instance.getPauseStatus() == false) {
                isAiming = true;
                if (gameManager.instance.getPlayerScript().getIsSniper() == false) {
                    if (snapZoom) {
                        cam.fieldOfView = aimingFOV; //if snap Zoom Enabled zoom instantly
                    }
                    else {
                        //if snap zoom not enabled, smooth zoom;
                        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, aimingFOV, zoomSpeed * Time.deltaTime);
                    }
                }
                else { // sniper zoom
                    cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, 20, 999);
                    sens = zoomSens;
                    gameManager.instance.scopeZoomIn();
                }
            }
        }
        else {
            isAiming = false;
            if (snapZoom) {
                cam.fieldOfView = normalFOV; //snap back to normal Fov
            }
            else {
                //smooth transition back to normal FoV
                cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, normalFOV, zoomSpeed * Time.deltaTime);
            }
            sens = startingSens;
            gameManager.instance.scopeZoomOut();
        }
    }

    void HandleLeaning()
    {
        float targetLeanAngle = 0f;
        Vector3 targetOffSet = OrigCameraPos; //orig camera start

        if (Input.GetButton("PeekLeft"))
        {
            targetLeanAngle = GetLeanAngle(); //lean left
            targetOffSet += Vector3.left * LeanOffSet; //off set camera lean left
            
        }
        else if (Input.GetButton("PeekRight"))
        {
            targetLeanAngle = -GetLeanAngle(); // lean right
            targetOffSet += Vector3.right * LeanOffSet; //off set camera lean right
        }

        currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, Time.deltaTime * leanSpeed); //smooth transition to lean

        transform.localRotation = Quaternion.Euler(rotX, transform.localEulerAngles.y, currentLeanAngle); //applies lean 

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetOffSet, Time.deltaTime * leanSpeed); // smooth lean offset
    }


    public void invert()
    {
       
        invertY = !invertY;
       

    }


    public void autoZoom()
    {
        snapZoom = !snapZoom;
    }
   
}
