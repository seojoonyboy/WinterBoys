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

        _eventManager = EventManager.Instance;
    }

    private void OnEnable() {
        animator.enabled = true;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("QTE", -1, 0);
    }

    public void OnClick() {
        animator.enabled = false;
        _eventManager.TriggerEvent(new SkiJump_QTE_end());
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
