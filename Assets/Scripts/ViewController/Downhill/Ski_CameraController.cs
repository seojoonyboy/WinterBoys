using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ski_CameraController : MonoBehaviour {
    public Transform target;
    private void FixedUpdate() {
        transform.position = new Vector3(target.position.x, target.transform.position.y - 0.8f, -3.5f);
    }
}
