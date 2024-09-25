using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadout : MonoBehaviour
{
    public static loadout instance;

    /*//Loadout items selected by player
    [SerializeField] ScriptableObject _primaryWeapon;
    [SerializeField] ScriptableObject _secondaryWeapon;
    [SerializeField] ScriptableObject _consumable;
    [SerializeField] ScriptableObject _consumable2;
    [SerializeField] ScriptableObject _throwable;*/

    [SerializeField] gunStats AR;
    [SerializeField] gunStats Sniper;
    [SerializeField] gunStats HandCannon;
    [SerializeField] gunStats Knife;
    [SerializeField] gunStats Health1;
    [SerializeField] gunStats Health2;
    [SerializeField] gunStats Gernade1;
    [SerializeField] gunStats Gernade2;

    List<gunStats> selectedGuns;

    //Players selected loadout
    /*List<ScriptableObject> selectedLoadout;*/
    /*List<ScriptableObject> presetLoadout1;
    List<ScriptableObject> presetLoadout2;*/

    /*public List<ScriptableObject> getPreset1() { return presetLoadout1; }
    public void setPreset1(List<ScriptableObject> _loadout)
    { presetLoadout1 = new List<ScriptableObject>();
      presetLoadout1.AddRange(_loadout);
    }
    public List<ScriptableObject> getPreset2() { return presetLoadout2; }
    public void setPreset2(List<ScriptableObject> _loadout)
    {
        presetLoadout2 = new List<ScriptableObject>();
        presetLoadout2.AddRange(_loadout);
    }*/
    /*public List<ScriptableObject> getSelectedLoadout() { return selectedLoadout; }
    public void setSelectedLoadout(List<ScriptableObject> _loadout)
    {
        selectedLoadout = new List<ScriptableObject>();
        selectedLoadout.AddRange(_loadout);
    }*/

    /*//Preset objects loadout 1
    [SerializeField] ScriptableObject _primaryLd1;
    [SerializeField] ScriptableObject _secondaryLd1;
    [SerializeField] ScriptableObject _cons1Ld1;
    [SerializeField] ScriptableObject _cons2Ld1;
    [SerializeField] ScriptableObject _thrwLd1;*/


    /*//Preset objects loadout 2
    [SerializeField] ScriptableObject _primaryLd2;
    [SerializeField] ScriptableObject _secondaryLd2;
    [SerializeField] ScriptableObject _cons1Ld2;
    [SerializeField] ScriptableObject _cons2Ld2;
    [SerializeField] ScriptableObject _thrwLd2;*/

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


        /*//Add preset loadout 1 items
        presetLoadout1.Add(_primaryLd1);
        presetLoadout1.Add(_secondaryLd1);
        presetLoadout1.Add(_cons1Ld1);
        presetLoadout1.Add(_cons2Ld1);
        presetLoadout1.Add(_thrwLd1);

        //Add preset loadout 2 items
        presetLoadout2.Add(_primaryLd2);
        presetLoadout2.Add(_secondaryLd2);
        presetLoadout2.Add(_cons1Ld2);
        presetLoadout2.Add(_cons2Ld2);
        presetLoadout2.Add(_thrwLd2);
*/

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

        getPrmLd2Img().gameObject.SetActive(true);
        getSecLd2Img().gameObject.SetActive(true);
        getCnsm1Ld2Img().gameObject.SetActive(true);
        getCnsm2Ld2Img().gameObject.SetActive(true);
        getThrwLd2Img().gameObject.SetActive(true);
        //playerMovement.player.getGunStats(Knife);
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
