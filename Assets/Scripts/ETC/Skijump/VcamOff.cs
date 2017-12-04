using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VcamOff : MonoBehaviour {
    private float time;
    private float maintainTime = 2.0f;
    private void Start() {
        time = 0;
    }

    private void Update() {
        time += Time.deltaTime;
        if(time >= maintainTime) {
            gameObject.SetActive(false);
        }
    }
}
