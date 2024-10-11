using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GunInventoryManager : MonoBehaviour
{
    [SerializeField] private List<Image> gunSlotImages; // List of UI images for gun slots
    [SerializeField] private List<Text> gunAmmoTexts;   // List of UI texts for ammo counts
    [SerializeField] private Color selectedGunColor;    // Color for the selected gun slot
    [SerializeField] private Color defaultGunColor;     // Default color for gun slots

    private playerMovement player;

    void Start()
    {
        // Initialize gun slots to empty and default color
        for (int i = 0; i < gunSlotImages.Count; i++)
        {
            gunSlotImages[i].color = defaultGunColor;
            gunSlotImages[i].sprite = null;  // Empty sprite (no gun)          
            gunAmmoTexts[i].text = "";
        }
    }

    public void UpdateGunInventoryUI()
    {
        List<gunStats> guns = player.getGunList();
        int selectedGunIndex = player.getGunList().IndexOf(player.getCurGun());

        for (int i = 0; i < gunSlotImages.Count; i++)
        {
            if (i < guns.Count)
            {
                // Set the sprite from the gunStats
                gunSlotImages[i].sprite = guns[i].gunSprite;

                // Update ammo text
                gunAmmoTexts[i].text = guns[i].ammoCur.ToString() + " / " + guns[i].ammoMag.ToString();

                // Highlight the selected gun
                if (i == selectedGunIndex)
                {
                    gunSlotImages[i].color = selectedGunColor;
                }
                else
                {
                    gunSlotImages[i].color = defaultGunColor;
                }
            }
            else
            {
                // Clear empty slots
                gunSlotImages[i].sprite = null;
                gunAmmoTexts[i].text = "";
            }
        }
    }
}