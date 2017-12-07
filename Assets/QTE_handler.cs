using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_handler : MonoBehaviour {
    private bool tmp = false;
    private float 
        startTime,
        time;
    private SkiJumpManager sm;
    private void Start() {
        sm = GameObject.FindGameObjectWithTag("GameController").GetComponent<SkiJumpManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (sm.isQTE_occured) { return; }

        tmp = true;
        Time.timeScale = 0.2f;

        startTime = Time.realtimeSinceStartup;

        sm.qteButton.SetActive(true);

        sm.isQTE_occured = true;
    }

    private void Update() {
        if (tmp) {
            time = Time.realtimeSinceStartup - startTime;

            //1.5초 이상이 지나면 QTE가 종료된다.
            if (time >= 1.5f) {
                Time.timeScale = 1.0f;
                tmp = false;

                sm.qteButton.SetActive(false);
            }
        }
    }
}
