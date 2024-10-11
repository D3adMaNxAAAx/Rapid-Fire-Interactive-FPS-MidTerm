using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInventoryManger : MonoBehaviour
{
    [SerializeField] private List<Image> gunSlotImages;
    [SerializeField] private List<Text> gunAmmoTexts;
    [SerializeField] private Color selectedGunColor;
    [SerializeField] private Color defaultGunColor;

    private playerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<playerMovement>();
    }

    // Update is called once per frame
    public void UpdateGunInventoryUI()
    {
        List<gunStats> guns = player.getGunList();  // Get the player's gun list
        int selectedGunIndex = player.getGunList().IndexOf(player.getCurGun());  // Get index of selected gun

        // Ensure no run into out-of-bounds issues
        for (int i = 0; i < gunSlotImages.Count; i++)
        {
            if (i < guns.Count)
            {
                // Get the gun's texture and convert to a sprite
                Texture2D texture = guns[i].gunModel.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
                if (texture != null)
                {
                    Rect rect = new Rect(0, 0, texture.width, texture.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f); // Set pivot to the center
                    gunSlotImages[i].sprite = Sprite.Create(texture, rect, pivot);
                }

                // Update ammo count
                gunAmmoTexts[i].text = guns[i].ammoCur.ToString() + " / " + guns[i].ammoMag.ToString();

                // Update slot color based on selection
                if (i == selectedGunIndex)
                {
                    gunSlotImages[i].color = selectedGunColor;  // Highlight selected gun
                }
                else
                {
                    gunSlotImages[i].color = defaultGunColor;  // Set default color for non-selected guns
                }
            }
            else
            {
              
                gunSlotImages[i].sprite = null;
                gunAmmoTexts[i].text = "";
                gunSlotImages[i].color = Color.clear;
            }
        }
    }
}
