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
    private int successCnt = 0;
    private bool isSuccess = false;
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
        if (isSuccess) {
            successCnt++;
        }
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

            sm.qte_magnification = successCnt * 0.1f;
            Debug.Log(sm.qte_magnification);
        }
        else {
            _eventManager.TriggerEvent(new SkiJump_QTE_end());
            Debug.Log("QTE 종료 트리거");
        }
    }

    public void SetState(int value) {
        if(value == 1) {
            isSuccess = true;
        }
        else {
            isSuccess = false;
        }
    }
}
