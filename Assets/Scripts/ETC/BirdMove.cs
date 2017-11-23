using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMove : MonoBehaviour {
    private void Update() {
        transform.Translate(Vector2.left * 0.1f);
    }
}
