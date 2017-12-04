﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour {
    public type _type;
    public enum type { NULL, SKIJUMP, DOWNHILL }
    private void Start() {
        InvokeRepeating("IsInArea", 1.0f, 1.0f);
    }

    void IsInArea() {
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        if(_type == type.SKIJUMP) {
            //Debug.Log(pos);
            if(pos.x <= -1.0f) {
                Destroy(gameObject);
            }
        }
        else if(_type == type.DOWNHILL) {
            if (pos.y >= 1.0f) {
                Destroy(gameObject);
            }
        }
    }
}