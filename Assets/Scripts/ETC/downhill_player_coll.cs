using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downhill_player_coll : MonoBehaviour {
    private DownhillManager dM;
    private bool isTriggered = false;
    private void Awake() {
        dM = GameObject.Find("Manager").GetComponent<DownhillManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Flag") {
            if (!isTriggered) {
                dM.remainTime -= (int)GameManager.Instance.panelty_time;
            }
            isTriggered = true;
        }
    }
}
