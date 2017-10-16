using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {
    public Transform target;

    void Update() {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 0.04f, target.transform.position.z);
    }
}
