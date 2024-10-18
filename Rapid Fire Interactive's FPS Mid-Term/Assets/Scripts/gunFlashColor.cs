using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class gunFlashColor : MonoBehaviour {

    public static gunFlashColor gunFlash; // singleton

    [SerializeField] GameObject flash;
    [SerializeField] Material Red;
    [SerializeField] Material Blue;
    [SerializeField] Material Green;
    [SerializeField] Material Orange;

    void Start() {
        gunFlash = this;
    }

    public void changeColor(int color) {
        switch (color) {
            case 1:
                flash.GetComponent<MeshRenderer>().sharedMaterial = Red;
                break;
            case 2:
                flash.GetComponent<MeshRenderer>().sharedMaterial = Blue;
                break;
            case 3:
                flash.GetComponent<MeshRenderer>().sharedMaterial = Green;
                break;
            case 4:
                flash.GetComponent<MeshRenderer>().sharedMaterial = Orange;
                break;
        }
    }
}
