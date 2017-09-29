using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp_CameraController : MonoBehaviour {
    public Transform target;
    private void FixedUpdate() {
        transform.position = new Vector3(target.position.x, 2.0f, target.position.z - 0.8f);
    }
}
