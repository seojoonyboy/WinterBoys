using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;

public class ArrowRotate : MonoBehaviour {
    private float limitTime = 1.0f;
    public float rotateAmount = 1;
    public bool canRotate;

    private Vector3 rotDir = Vector3.forward;
    private int rotCnt = 0;

    public AudioSource extraAudioSource;
    public Slider slider;
    private void Start() {
        canRotate = false;
    }

    private void Update() {
        limitTime -= Time.fixedUnscaledDeltaTime;
        if (canRotate) {
            if(transform.eulerAngles.z < 90) {
                transform.Rotate(Vector3.forward * 2f);
            }
        }

        if (limitTime < 0) {
            OnPointerUp();
        }

        slider.value = transform.eulerAngles.z;
    }

    public void OnPointerDown() {
        canRotate = true;
    }

    public void OnPointerUp() {
        EventManager.Instance.TriggerEvent(new SkiJump_ArrowRotEndEvent());
        canRotate = false;

        extraAudioSource.Stop();
    }
}
