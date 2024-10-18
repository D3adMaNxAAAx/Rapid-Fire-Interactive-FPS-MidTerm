using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keepSettings : MonoBehaviour
{
    public static keepSettings gameSettings;

    [SerializeField] GameObject settingMenu;
    //[SerializeField] bool inverBool;
    //[SerializeField] GameObject togSprinBool;
    //[SerializeField] GameObject togTimeBool;
    //[SerializeField] GameObject togFocus;
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
        
    }
    public void setDefaults()
    {
        settingMenu = GameObject.Find("Settings Menu");
        if (settingMenu != null)
        {
            sensSlider = GameObject.Find("Sensitivity ").GetComponent<Slider>();
            musicSlider = GameObject.Find("Volume (Music)").GetComponent<Slider>();
            sfxVolume = GameObject.Find("Volume (SFX)").GetComponent<Slider>();
            sensSlider.value = sens;
            musicSlider.value = mus;
            sfxVolume.value = sfx;
            GameObject.Find("Invert Y").GetComponent<Toggle>().isOn = invertY;
            GameObject.Find("Toggle Sprint").GetComponent<Toggle>().isOn = sprintToggle;
            GameObject.Find("Toggle Timer").GetComponent<Toggle>().isOn = timerTog;
            GameObject.Find("Toggle Focus").GetComponent<Toggle>().isOn = focusTog;
        }
        else
            return;
    }

    public void settingUpdate()
    {
        
    }
    public void inverBool(bool tick)
    {
        invertY = tick;
    }
    public void togSprinBool(bool tick)
    {
        sprintToggle = tick;
    }
    public void togTimeBool(bool tick)
    {
        timerTog = tick;
    }
    public void togFocus(bool tick)
    {
        focusTog = tick;
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
}
