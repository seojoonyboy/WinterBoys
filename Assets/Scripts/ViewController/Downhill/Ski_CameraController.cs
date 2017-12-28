using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ski_CameraController : MonoBehaviour {
    public Transform target;
    private void FixedUpdate() {
        transform.position = new Vector3(transform.position.x, target.transform.position.y - 1.8f, -9.74f);
    }
}
