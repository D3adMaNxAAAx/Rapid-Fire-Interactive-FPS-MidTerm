using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInventoryManager : MonoBehaviour
{
    [SerializeField]  List<Image> gunSlotImages;

    private void Update()
    {
        if (gameManager.instance.getMenuLoadout().activeInHierarchy == false) ;
        {
            UpdateGunInventoryUI();
            hotkeyelection();
        }
    }

    public void UpdateGunInventoryUI()
    {
            List<gunStats> guns = gameManager.instance.getPlayerScript().getGunList();
     
            for (int i = 0; i < guns.Count; i++)
            {
                gunSlotImages[i].sprite = guns[i].gunIcon;
            }
    }

    public void hotkeyelection()
    {
        if (Input.GetButtonDown("Hotkey1"))
        { gameManager.instance.getPlayerScript().setCurrGun(0); }
    }
}