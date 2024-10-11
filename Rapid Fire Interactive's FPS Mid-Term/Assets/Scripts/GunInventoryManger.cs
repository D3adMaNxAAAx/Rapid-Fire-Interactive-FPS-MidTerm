using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInventoryManager : MonoBehaviour
{
    [SerializeField] private List<Image> gunSlotImages;
    [SerializeField] private List<Sprite> gunSlotSprites;    
    [SerializeField] private Color selectedGunColor;
    [SerializeField] private Color defaultGunColor;

    private playerMovement player;


    void Start()
    {
        player = FindObjectOfType<playerMovement>();

        for (int i = 0; i < gunSlotImages.Count; i++)
        {
            gunSlotImages[i].color = defaultGunColor;
            gunSlotImages[i].sprite = null;

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
                gunSlotImages[i].sprite = guns[i].gunIcon;  
                gunSlotImages[i].color = i == selectedGunIndex ? selectedGunColor : defaultGunColor; 
            }
            else
            {
                
                gunSlotImages[i].sprite = null;
                gunSlotImages[i].color = defaultGunColor;
            }
        }
    }
}