using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using UnityEngine.UI;

public class QTE_button_handler : MonoBehaviour {
    public SkiJumpManager sm;
    private Animator animator;
    private EventManager _eventManager;

    private RectTransform rect;

    private int qteCnt = 0;
    private void Awake() {
        animator = GetComponent<Animator>();

        _eventManager = EventManager.Instance;
        rect = transform.parent.GetComponent<RectTransform>();
    }

    private void OnEnable() {
        qteCnt = 0;
        nextQTE();
    }

    public void OnClick() {
        nextQTE();
    }

    private void nextQTE() {
        qteCnt++;

        float randX = Random.Range(-780, 780);
        float randY = Random.Range(-330, 330);

        rect.localPosition = new Vector3(randX, randY, 0);

        if(qteCnt <= 3) {
            animator.enabled = true;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.Play("QTE", -1, 0);
        }
        else {
            _eventManager.TriggerEvent(new SkiJump_QTE_end());
        }
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
