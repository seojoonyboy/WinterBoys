using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class ArrowRotate : MonoBehaviour {
    private Rigidbody2D rb;
    private float limitTime = 1.0f;
    public float rotateAmount = 1;
    public bool canRotate;

    private Vector3 rotDir = Vector3.forward;
    private int rotCnt = 0;

    public AudioSource extraAudioSource;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        canRotate = false;
    }

    private void Update() {
        limitTime -= Time.fixedUnscaledDeltaTime;
        Debug.Log(transform.right);
        if (canRotate) {
            if(transform.eulerAngles.z < 90) {
                transform.Rotate(Vector3.forward * 2f);
            }
        }

        if (limitTime < 0) {
            OnPointerUp();
        }
    }

    public void OnPointerDown() {
        canRotate = true;
    }

    public void OnPointerUp() {
        Debug.Log("OnPointerUp");
        EventManager.Instance.TriggerEvent(new SkiJump_ArrowRotEndEvent());
        canRotate = false;

        extraAudioSource.Stop();
    }
}
