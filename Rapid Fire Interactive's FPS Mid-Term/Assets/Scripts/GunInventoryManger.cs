using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunInventoryManager : MonoBehaviour
{
    [SerializeField]  List<Image> gunSlotImages;
    [SerializeField]  List<TextMeshProUGUI> hotkeyTexts;
    private void Update()
    {
        if (gameManager.instance.getMenuLoadout().activeInHierarchy == false)
        {
            UpdateGunInventoryUI();
            hotkeyelection();
        }
    }

    public void UpdateGunInventoryUI()
    {
            List<gunStats> guns = playerMovement.player.getGunList();
     
            for (int i = 0; i < guns.Count; i++)
            {
                gunSlotImages[i].sprite = guns[i].gunIcon;
            }
    }

    public void hotkeyelection()
    {
        if (Input.GetButtonDown("Hotkey1"))
        { playerMovement.player.setCurrGun(0); }
        if (Input.GetButtonDown("Hotkey2"))
        { playerMovement.player.setCurrGun(1); }
        if (Input.GetButtonDown("Hotkey3"))
        { playerMovement.player.setCurrGun(2); }
        if (Input.GetButtonDown("Hotkey4"))
        { playerMovement.player.setCurrGun(3); }
        if (Input.GetButtonDown("Hotkey5"))
        { playerMovement.player.setCurrGun(4); }
        if (Input.GetButtonDown("Hotkey6"))
        { playerMovement.player.setCurrGun(5); }
        if (Input.GetButtonDown("Hotkey7"))
        { playerMovement.player.setCurrGun(6); }
        if (Input.GetButtonDown("Hotkey8"))
        { playerMovement.player.setCurrGun(7); }
        if (Input.GetButtonDown("Hotkey9"))
        { playerMovement.player.setCurrGun(8); }
        if (Input.GetButtonDown("Hotkey10"))
        { playerMovement.player.setCurrGun(9); }
    }
}