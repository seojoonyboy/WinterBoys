using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixEffect : MonoBehaviour {
    public Transform target;

    private void Update() {
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(target.position.x, target.position.y + 0.46f, 0.2f);
    }
}
