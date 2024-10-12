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
        if (Input.GetButtonDown("Hotkey2"))
        { gameManager.instance.getPlayerScript().setCurrGun(1); }
        if (Input.GetButtonDown("Hotkey3"))
        { gameManager.instance.getPlayerScript().setCurrGun(2); }
        if (Input.GetButtonDown("Hotkey4"))
        { gameManager.instance.getPlayerScript().setCurrGun(3); }
        if (Input.GetButtonDown("Hotkey5"))
        { gameManager.instance.getPlayerScript().setCurrGun(4); }
        if (Input.GetButtonDown("Hotkey6"))
        { gameManager.instance.getPlayerScript().setCurrGun(5); }
        if (Input.GetButtonDown("Hotkey7"))
        { gameManager.instance.getPlayerScript().setCurrGun(6); }
        if (Input.GetButtonDown("Hotkey8"))
        { gameManager.instance.getPlayerScript().setCurrGun(7); }
        if (Input.GetButtonDown("Hotkey9"))
        { gameManager.instance.getPlayerScript().setCurrGun(8); }
        if (Input.GetButtonDown("Hotkey10"))
        { gameManager.instance.getPlayerScript().setCurrGun(9); }
    }
}