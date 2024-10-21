using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class loadout : MonoBehaviour {

    /// need to make it so that player can't start game without selecting a weapon loadout

    public static loadout instance;

    [SerializeField] gunStats AR;
    [SerializeField] gunStats Sniper;
    [SerializeField] gunStats HandCannon;
    [SerializeField] gunStats Shotgun;
    [SerializeField] HealStats Health1;
    [SerializeField] HealStats Health2;
    [SerializeField] GrenadeStats Gernade1;
    [SerializeField] GrenadeStats Gernade2;

    //Preset objects loadout 1 images
    [SerializeField] Image _primaryLd1Image;
    [SerializeField] TMP_Text _primaryLd1Name;
    [SerializeField] Image _secondaryLd1Image;
    [SerializeField] TMP_Text _secondaryLd1Name;
    [SerializeField] Image _cons1Ld1Image;
    [SerializeField] TMP_Text _cons1Ld1Name;
    [SerializeField] Image _cons2Ld1Image;
    [SerializeField] TMP_Text _cons2Ld1Name;
    [SerializeField] Image _thrwLd1Image;
    [SerializeField] TMP_Text _thrwLd1Name;

    //Preset objects loadout 2 images
    [SerializeField] Image _primaryLd2Image;
    [SerializeField] TMP_Text _primaryLd2Name;
    [SerializeField] Image _secondaryLd2Image;
    [SerializeField] TMP_Text _secondaryLd2Name;
    [SerializeField] Image _cons1Ld2Image;
    [SerializeField] TMP_Text _cons1Ld2Name;
    [SerializeField] Image _cons2Ld2Image;
    [SerializeField] TMP_Text _cons2Ld2Name;
    [SerializeField] Image _thrwLd2Image;
    [SerializeField] TMP_Text _thrwLd2Name;

    bool isSelected = false;

    public Image getPrmLd1Img() { return _primaryLd1Image; }

    public void setPrmLd1Img(Image _src) { _primaryLd1Image = _src; }

    public Image getSecLd1Img() { return _secondaryLd1Image; }

    public void setSecLd1Img(Image _src) { _secondaryLd1Image = _src; }

    public Image getCnsm1Ld1Img() { return _cons1Ld1Image; }

    public void setCnsm1Ld1Img(Image _src) { _cons1Ld1Image = _src; }

    public Image getCnsm2Ld1Img() { return _cons2Ld1Image; }

    public void setCnsm2Ld1Img(Image _src) { _cons2Ld1Image = _src; }
    public Image getThrwLd1Img() { return _thrwLd1Image; }

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

    public void loadOut1 () {

        isSelected = true;

        playerMovement.player.removeFromGrenades();
        playerMovement.player.removeFromHeals();

        playerMovement.player.getGunList().Clear();

        playerMovement.player.getGunStats(AR);
        playerMovement.player.getGunStats(HandCannon);

        playerMovement.player.addToGrenades(Gernade1);
        playerMovement.player.addToHeals(Health1);
        playerMovement.player.addToHeals(Health2);

        getPrmLd2Img().gameObject.SetActive(false);
        _primaryLd2Name.gameObject.SetActive(false);
        getSecLd2Img().gameObject.SetActive(false);
        _secondaryLd2Name.gameObject.SetActive(false);
        getCnsm1Ld2Img().gameObject.SetActive(false);
        _cons1Ld2Name.gameObject.SetActive(false);
        getCnsm2Ld2Img().gameObject.SetActive(false);
        _cons2Ld2Name.gameObject.SetActive(false);
        getThrwLd2Img().gameObject.SetActive(false);
        _thrwLd2Name.gameObject.SetActive(false);

        getPrmLd1Img().gameObject.SetActive(true);
        _primaryLd1Name.gameObject.SetActive(true);
        getSecLd1Img().gameObject.SetActive(true);
        _secondaryLd1Name.gameObject.SetActive(true);
        getCnsm1Ld1Img().gameObject.SetActive(true);
        _cons1Ld1Name.gameObject.SetActive(true);
        getCnsm2Ld1Img().gameObject.SetActive(true);
        _cons2Ld1Name.gameObject.SetActive(true);
        getThrwLd1Img().gameObject.SetActive(true);
        _thrwLd1Name.gameObject.SetActive(true);
    }

    public void loadOut1HoverEnter()
    {
        getPrmLd2Img().gameObject.SetActive(false);
        _primaryLd2Name.gameObject.SetActive(false);
        getSecLd2Img().gameObject.SetActive(false);
        _secondaryLd2Name.gameObject.SetActive(false);
        getCnsm1Ld2Img().gameObject.SetActive(false);
        _cons1Ld2Name.gameObject.SetActive(false);
        getCnsm2Ld2Img().gameObject.SetActive(false);
        _cons2Ld2Name.gameObject.SetActive(false);
        getThrwLd2Img().gameObject.SetActive(false);
        _thrwLd2Name.gameObject.SetActive(false);

        getPrmLd1Img().gameObject.SetActive(true);
        _primaryLd1Name.gameObject.SetActive(true);
        getSecLd1Img().gameObject.SetActive(true);
        _secondaryLd1Name.gameObject.SetActive(true);
        getCnsm1Ld1Img().gameObject.SetActive(true);
        _cons1Ld1Name.gameObject.SetActive(true);
        getCnsm2Ld1Img().gameObject.SetActive(true);
        _cons2Ld1Name.gameObject.SetActive(true);
        getThrwLd1Img().gameObject.SetActive(true);
        _thrwLd1Name.gameObject.SetActive(true);

       
    }
    public void loadOut1HoverExit()
    {
        if (!isSelected)
        {
            getPrmLd1Img().gameObject.SetActive(false);
            _primaryLd1Name.gameObject.SetActive(false);
            getSecLd1Img().gameObject.SetActive(false);
            _secondaryLd1Name.gameObject.SetActive(false);
            getCnsm1Ld1Img().gameObject.SetActive(false);
            _cons1Ld1Name.gameObject.SetActive(false);
            getCnsm2Ld1Img().gameObject.SetActive(false);
            _cons2Ld1Name.gameObject.SetActive(false);
            getThrwLd1Img().gameObject.SetActive(false);
            _thrwLd1Name.gameObject.SetActive(false);
        }else return;
    }

        public void loadOut2() {
        isSelected = true;
        playerMovement.player.removeFromGrenades();
        playerMovement.player.removeFromHeals();

        playerMovement.player.getGunList().Clear();

        playerMovement.player.getGunStats(Sniper);
        playerMovement.player.getGunStats(Shotgun);

        getPrmLd1Img().gameObject.SetActive(false);
        _primaryLd1Name.gameObject.SetActive(false);
        getSecLd1Img().gameObject.SetActive(false);
        _secondaryLd1Name.gameObject.SetActive(false);
        getCnsm1Ld1Img().gameObject.SetActive(false);
        _cons1Ld1Name.gameObject.SetActive(false);
        getCnsm2Ld1Img().gameObject.SetActive(false);
        _cons2Ld1Name.gameObject.SetActive(false);
        getThrwLd1Img().gameObject.SetActive(false);
        _thrwLd1Name.gameObject.SetActive(false);

        getPrmLd2Img().gameObject.SetActive(true);
        _primaryLd2Name.gameObject.SetActive(true);
        getSecLd2Img().gameObject.SetActive(true);
        _secondaryLd2Name.gameObject.SetActive(true);
        getCnsm1Ld2Img().gameObject.SetActive(true);
        _cons1Ld2Name.gameObject.SetActive(true);
        getCnsm2Ld2Img().gameObject.SetActive(true);
        _cons2Ld2Name.gameObject.SetActive(true);
        getThrwLd2Img().gameObject.SetActive(true);
        _thrwLd2Name.gameObject.SetActive(true);

        playerMovement.player.addToGrenades(Gernade1);
        playerMovement.player.addToGrenades(Gernade2);
        playerMovement.player.addToHeals(Health1);
    }

    public void loadOut2HoverEnter()
    {
        getPrmLd1Img().gameObject.SetActive(false);
        _primaryLd1Name.gameObject.SetActive(false);
        getSecLd1Img().gameObject.SetActive(false);
        _secondaryLd1Name.gameObject.SetActive(false);
        getCnsm1Ld1Img().gameObject.SetActive(false);
        _cons1Ld1Name.gameObject.SetActive(false);
        getCnsm2Ld1Img().gameObject.SetActive(false);
        _cons2Ld1Name.gameObject.SetActive(false);
        getThrwLd1Img().gameObject.SetActive(false);
        _thrwLd1Name.gameObject.SetActive(false);

        getPrmLd2Img().gameObject.SetActive(true);
        _primaryLd2Name.gameObject.SetActive(true);
        getSecLd2Img().gameObject.SetActive(true);
        _secondaryLd2Name.gameObject.SetActive(true);
        getCnsm1Ld2Img().gameObject.SetActive(true);
        _cons1Ld2Name.gameObject.SetActive(true);
        getCnsm2Ld2Img().gameObject.SetActive(true);
        _cons2Ld2Name.gameObject.SetActive(true);
        getThrwLd2Img().gameObject.SetActive(true);
        _thrwLd2Name.gameObject.SetActive(true);
    }
    public void loadOut2HoverExit()
    {
        if (!isSelected)
        {
            getPrmLd2Img().gameObject.SetActive(false);
            _primaryLd2Name.gameObject.SetActive(false);
            getSecLd2Img().gameObject.SetActive(false);
            _secondaryLd2Name.gameObject.SetActive(false);
            getCnsm1Ld2Img().gameObject.SetActive(false);
            _cons1Ld2Name.gameObject.SetActive(false);
            getCnsm2Ld2Img().gameObject.SetActive(false);
            _cons2Ld2Name.gameObject.SetActive(false);
            getThrwLd2Img().gameObject.SetActive(false);
            _thrwLd2Name.gameObject.SetActive(false);
        }
    }

    

}
