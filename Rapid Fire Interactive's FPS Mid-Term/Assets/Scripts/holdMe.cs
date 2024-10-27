using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdMe : MonoBehaviour
{
    public static holdMe instance;
    [SerializeField] List<GameObject> iDs;
    [SerializeField] List<GameObject> pwrItems;


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


}
