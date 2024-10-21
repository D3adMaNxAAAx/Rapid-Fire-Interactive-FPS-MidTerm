using UnityEngine;
using UnityEngine.UI;


public class StatusEffectUIManager : MonoBehaviour
{
    [Header("Status Effect Icons")]
    public Image burningIcon;    // Drag your Burning UI Image here
    public Image bleedingIcon;   // Drag your Bleeding UI Image here

    private void Start()
    {
        // Ensure both icons are hidden when the game starts
        burningIcon.gameObject.SetActive(false);
        bleedingIcon.gameObject.SetActive(false);
    }

    // Show the burning icon
    public void ShowBurningEffect()
    {
        Debug.Log("Showing Burning Effect UI");
        burningIcon.gameObject.SetActive(true);
    }

    // Hide the burning icon
    public void HideBurningEffect()
    {
        Debug.Log("Hiding Burning Effect UI");
        burningIcon.gameObject.SetActive(false);
    }

    // Show the bleeding icon
    public void ShowBleedingEffect()
    {
        Debug.Log("Showing Bleeding Effect UI");
        bleedingIcon.gameObject.SetActive(true);
    }

    // Hide the bleeding icon
    public void HideBleedingEffect()
    {
        Debug.Log("Hiding Bleeding Effect UI");
        bleedingIcon.gameObject.SetActive(false);
    }
}