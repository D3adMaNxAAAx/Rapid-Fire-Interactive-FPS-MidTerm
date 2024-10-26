using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdMe : MonoBehaviour
{
    public static holdMe instance;
    [SerializeField] List<GameObject> iDs;
    [SerializeField] List<GameObject> pwrItems;
    [SerializeField] GameObject terminalMainMenu;
    [SerializeField] GameObject terminalUpgradeMenu;

    private void Awake()
    {
        instance = this;
    }
    public List<GameObject> getIDImages()
    {
        return iDs;
    }
    public List<GameObject> getPwrItems()
    {
        return pwrItems;
    }
    

    public GameObject getTerminalMainMenu() { return terminalMainMenu; }
    public GameObject getTerminalUpgradeMenu() { return terminalUpgradeMenu; }
}
