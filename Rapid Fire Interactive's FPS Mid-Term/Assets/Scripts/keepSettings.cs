using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keepSettings : MonoBehaviour
{
    public static keepSettings gameSettings;

    [SerializeField] GameObject settingMenu;
    [SerializeField] Toggle inverBool;
    [SerializeField] Toggle togSprinBool;
    [SerializeField] Toggle togTimeBool;
    [SerializeField] Toggle togFocus;
    [SerializeField] Slider sensSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxVolume;

    bool invertY = false;
    bool sprintToggle = false;
    bool timerTog = true;
    bool focusTog = true;
    float sens = 300f;
    float mus = 1f;
    float sfx = 1f;
    GameObject UIobj;

    private void Awake()
    {

        if (gameSettings == null)
        {
            gameSettings = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
        
        setDefaults();
        settingMenu.SetActive(false);
    }
    private void Update()
    {
        if (settingMenu == null)
        {
            setDefaults();
        }
        else
        {
            getSettingsUpdate();
            settingUpdate();
        }
        if(UIobj == null)
        {
            UIobj = GameObject.Find("UI");
        }
        
    }
    public void setDefaults()
    {
        settingMenu = GameObject.Find("Settings Menu");
        if (settingMenu != null)
        {
            inverBool = GameObject.Find("Invert Y").GetComponent<Toggle>();
            togSprinBool = GameObject.Find("Toggle Sprint").GetComponent<Toggle>();
            togTimeBool = GameObject.Find("Toggle Timer").GetComponent<Toggle>();
            togFocus = GameObject.Find("Toggle Focus").GetComponent<Toggle>();
            sensSlider = GameObject.Find("Sensitivity ").GetComponent<Slider>();
            musicSlider = GameObject.Find("Volume (Music)").GetComponent<Slider>();
            sfxVolume = GameObject.Find("Volume (SFX)").GetComponent<Slider>();
            inverBool.isOn = invertY;
            togSprinBool.isOn = sprintToggle;
            togTimeBool.isOn = timerTog;
            togFocus.isOn = focusTog;
            sensSlider.value = sens;
            musicSlider.value = mus;
            sfxVolume.value = sfx;
            settingMenu.SetActive(false);
        }
        else
            return;
    }

    public void settingUpdate()
    {
        if (CameraMovement.state != null)
        {
            CameraMovement.state.SetSens(sens);
            CameraMovement.state.SetInvertY(invertY);
            CameraMovement.state.setZoomeSnap(focusTog);
        }
        if (playerMovement.player != null)
        {
            playerMovement.player.setSprintBool(sprintToggle);
        }
    }

    private void getSettingsUpdate()
    {
        invertY = inverBool.isOn;
        sprintToggle = togSprinBool.isOn;
        timerTog = togTimeBool.isOn;
        focusTog = togFocus.isOn;
        sens = sensSlider.value;
        mus = musicSlider.value;
        sfx = sfxVolume.value;
    }
    public void setSens(float value)
    {
        sens = value;
    }
    public void setMus(float value)
    {
        mus = value;
    }
    public void setSFX(float value)
    {
        sfx = value;
    }
    public bool getInvert()
    {
        return invertY;
    }
    public bool getSprint()
    {
        return sprintToggle;
    }
    public bool getTimer()
    {
        return timerTog;
    }
    public bool getFocus()
    {
        return focusTog;
    }
    public float getSens()
    {
        return sens;
    }
    public float getMus()
    { 
        return mus;
    }
    public float getSFX()
    {
        return sfx;
    }
    public GameObject getUIobj()
    {
        return UIobj;
    }
}
