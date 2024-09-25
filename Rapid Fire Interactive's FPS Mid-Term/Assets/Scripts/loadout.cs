using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadout : MonoBehaviour
{
    public static loadout instance;

    [SerializeField] gunStats AR;
    [SerializeField] gunStats Sniper;
    [SerializeField] gunStats HandCannon;
    [SerializeField] gunStats Pistol;
    [SerializeField] gunStats Health1;
    [SerializeField] gunStats Health2;
    [SerializeField] GrenadeStats Gernade1;
    [SerializeField] GrenadeStats Gernade2;

    //Preset objects loadout 1 images
    [SerializeField] Image _primaryLd1Image;
    [SerializeField] Image _secondaryLd1Image;
    [SerializeField] Image _cons1Ld1Image;
    [SerializeField] Image _cons2Ld1Image;
    [SerializeField] Image _thrwLd1Image;

    //Preset objects loadout 2 images
    [SerializeField] Image _primaryLd2Image;
    [SerializeField] Image _secondaryLd2Image;
    [SerializeField] Image _cons1Ld2Image;
    [SerializeField] Image _cons2Ld2Image;
    [SerializeField] Image _thrwLd2Image;

    public Image getPrmLd1Img() { return _primaryLd1Image; }

    public void setPrmLd1Img(Image _src) { _primaryLd1Image = _src; }

    public Image getSecLd1Img() { return _secondaryLd1Image; }

    public void setSecLd1Img(Image _src) { _secondaryLd1Image = _src; }

    public Image getCnsm1Ld1Img() { return _cons1Ld1Image; }

    public void setCnsm1Ld1Img(Image _src) { _cons1Ld1Image = _src; }

    public Image getCnsm2Ld1Img() { return _cons2Ld1Image; }

    public void setCnsm2Ld1Img(Image _src) { _cons2Ld1Image = _src; }
    public Image getThrwLd1Img() { return _primaryLd1Image; }

    public void setThrwLd1Img(Image _src) { _thrwLd1Image = _src; }


    public Image getPrmLd2Img() { return _primaryLd2Image; }

    public void setPrmLd2Img(Image _src) { _primaryLd2Image = _src; }

    public Image getSecLd2Img() { return _secondaryLd2Image; }

    public void setSecLd2Img(Image _src) { _secondaryLd2Image = _src; }

    public Image getCnsm1Ld2Img() { return _cons1Ld2Image; }

    public void setCnsm1Ld2Img(Image _src) { _cons1Ld2Image = _src; }

    public Image getCnsm2Ld2Img() { return _cons2Ld2Image; }

    public void setCnsm2Ld2Img(Image _src) { _cons2Ld2Image = _src; }

    public Image getThrwLd2Img() { return _thrwLd2Image; }

    public void setThrwLd2Img(Image _src) { _thrwLd2Image = _src; }


    /*//Lists for players to select items out of 
    [SerializeField] List<ScriptableObject> primaryWeapons; 
    [SerializeField] List<ScriptableObject> secondaryWeapons;
    [SerializeField] List<ScriptableObject> consumables;
    [SerializeField] List<ScriptableObject> throwables;*/

    // Start is called before the first frame update
    void Start() {
        instance = this;
    }

    public void imageOn(Image _src)
        { _src.enabled = true; }
    public void imageOff(Image _src)
        { _src.enabled = false; }

    public void loadOut1 () {
        playerMovement.player.getGunStats(AR);
        playerMovement.player.getGunStats(HandCannon);

        /*playerMovement.player.getGunStats(Gernade1);
        playerMovement.player.getGunStats(Health1);
        playerMovement.player.getGunStats(Health2);*/

        imageOn(getPrmLd1Img());
        imageOn(getSecLd1Img());
        imageOn(getCnsm1Ld1Img());
        imageOn(getCnsm2Ld1Img());
        imageOn(getThrwLd1Img());

    }

    public void loadOut2() {
        playerMovement.player.getGunStats(Sniper);
        playerMovement.player.getGunStats(Pistol);

        getPrmLd2Img().gameObject.SetActive(true);
        getSecLd2Img().gameObject.SetActive(true);
        getCnsm1Ld2Img().gameObject.SetActive(true);
        getCnsm2Ld2Img().gameObject.SetActive(true);
        getThrwLd2Img().gameObject.SetActive(true);

        /*playerMovement.player.getGunStats(Gernade1);
        playerMovement.player.getGunStats(Gernade2);
        playerMovement.player.getGunStats(Health1);*/

    }

    //click on the primary weapon slot
    //list of weapons show for primarys 
    //same for all other choices 
    //need a choose primary, seconday, consumable, and throwable button
    //already have back button 
    //loadout 1 button adds all loadout 1 preset scriptable weapons, cons, throws
    //same for loadout 2 button 

    //Confirm Loadout button sets loadout and starts game 
    //Quit game button as well 

}
