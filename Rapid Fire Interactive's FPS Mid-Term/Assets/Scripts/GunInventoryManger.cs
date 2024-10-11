using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInventoryManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> gunSlotSprites; 
    [SerializeField] private List<Text> gunAmmoTexts;
    [SerializeField] private Color selectedGunColor;
    [SerializeField] private Color defaultGunColor;

    private playerMovement player;


    void Start()
    {
        for (int i = 0; i < gunSlotSprites.Count; i++)
        {
           
            gunSlotSprites[i] = null;
            gunAmmoTexts[i].text = "";
        }
    }

    public void UpdateGunInventoryUI()
    {
        List<gunStats> guns = player.getGunList();
        int selectedGunIndex = player.getGunList().IndexOf(player.getCurGun());

        for (int i = 0; i < gunSlotSprites.Count; i++)
        {
            if (i < guns.Count)
            {
                
                gunSlotSprites[i] = guns[i].gunIcon; 

                // Update the ammo count text
                gunAmmoTexts[i].text = guns[i].ammoCur.ToString() + " / " + guns[i].ammoMag.ToString();
            }
            else
            {
                // Set to default empty sprite if no gun is present
                gunSlotSprites[i] = null;
                gunAmmoTexts[i].text = "";
            }
        }
    }
}