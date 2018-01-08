using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class QTE_button_handler : MonoBehaviour {
    public SkiJumpManager sm;
    private Animator animator;
    private EventManager _eventManager;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        _eventManager = EventManager.Instance;
        _eventManager.AddListener<SkiJump_Resume>(resume);
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

    private void resume(SkiJump_Resume e) {
        if (GetComponent<Animator>() == null) { return; }
        animator = GetComponent<Animator>();
        animator.enabled = true;
    }
}
