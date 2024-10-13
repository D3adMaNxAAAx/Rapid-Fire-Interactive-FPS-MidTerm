using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timer : MonoBehaviour {

    // TMP_Text timerText; in gameManager
    float currentTime = 0;
    TimeSpan time; // will convert long float decimal to just seconds

    // Update is called once per frame
    void Update() {
        currentTime = currentTime + Time.deltaTime; // adding how much timer passed during frame
        // time will automatically stop when paused because it is using time and TimeScale is set to 0 on pause
        time = TimeSpan.FromSeconds(currentTime);
        if (time.Minutes == 0) {
            gameManager.instance.getTimerText().text = time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
        else {
            gameManager.instance.getTimerText().text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
        playerStats.Stats.currentTime(gameManager.instance.getTimerText().text);
    }
}
