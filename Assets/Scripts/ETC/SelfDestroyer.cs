using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour {

    private void Start() {
        InvokeRepeating("IsInArea", 1.0f, 1.0f);
    }

    void IsInArea() {
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        if (pos.y >= 1.0f) {
            Destroy(gameObject);
        }
    }
}
