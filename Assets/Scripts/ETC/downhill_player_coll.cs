using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downhill_player_coll : MonoBehaviour {
    private DownhillManager dM;
    private void Awake() {
        dM = GameObject.Find("Manager").GetComponent<DownhillManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Flag") {
            dM.remainTime -= (int)GameManager.Instance.panelty_time;
        }
        Debug.Log("충돌 : " + other.tag);
    }
}
