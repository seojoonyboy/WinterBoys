﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_button_handler : MonoBehaviour {
    public SkiJumpManager sm;
    private Animator animator;

    private void OnEnable() {
        animator = GetComponent<Animator>();
    }

    public void OnClick() {
        animator.enabled = false;
        Invoke("off", 0.5f);
    }

    public void off() {
        //gameObject.SetActive(false);
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }

    public void setQteScore(int index) {
        float value = 0;
        switch (index) {
            case 0:
                value = 0;
                break;
            case 1:
                value = 0.1f;
                break;
            case 2:
                value = 0.2f;
                break;
            case 3:
                value = 0.3f;
                break;
        }
        sm.qte_magnification = value;
    }
}
