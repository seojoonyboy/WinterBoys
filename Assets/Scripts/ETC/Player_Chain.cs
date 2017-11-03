using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Chain : MonoBehaviour {
    public Transform character;

    private void Update() {
        transform.position = character.position;
        transform.rotation = character.rotation;
    }
}
