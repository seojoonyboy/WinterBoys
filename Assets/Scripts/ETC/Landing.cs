using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour {
    public delegate void EnterColliderHandler();
    public static event EnterColliderHandler OnLanding;

    private void OnCollisionEnter2D(Collision2D collision) {
        OnLanding();
    }
}
