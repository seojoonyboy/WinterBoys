using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDestroyer : MonoBehaviour {

    void Start() {
        InvokeRepeating("check", 1.0f, 1.0f);
    }

    void check() {
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        if(pos.y >= 1.0f) {
            Destroy(gameObject);
        }
    }
}
