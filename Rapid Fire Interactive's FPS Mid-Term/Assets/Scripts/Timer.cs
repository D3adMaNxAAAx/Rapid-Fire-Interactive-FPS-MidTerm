using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timer : MonoBehaviour {

    [SerializeField] TMP_Text timeText;
    float currentTime = 0;
    TimeSpan time; // will convert long float decimal to just seconds

    // Update is called once per frame
    void Update() {
        currentTime = currentTime + Time.deltaTime; // adding how much timer passed during frame
        // time will automatically stop when paused because it is using time and TimeScale is set to 0 on pause
        time = TimeSpan.FromSeconds(currentTime);
        if (time.Minutes == 0) {
            timeText.text = time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
        else {
            timeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
    }
}
