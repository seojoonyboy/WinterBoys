using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using UnityEngine.UI;
using Spine.Unity;

public class QTE_button_handler : MonoBehaviour {
    public SkiJumpManager sm;
    private Animator animator;
    private EventManager _eventManager;

    private RectTransform rect;

    private int qteCnt = 0;
    private int successCnt = 0;
    private bool isSuccess = false;

    public GameObject
        blur,
        QTEResults,
        successObj,
        failObj;

    private void Awake() {
        animator = GetComponent<Animator>();

        _eventManager = EventManager.Instance;
    }

    private void OnEnable() {
        rect = transform.parent.GetComponent<RectTransform>();
        blur.SetActive(true);
    }

    private void OnDisable() {
        blur.SetActive(false);
    }

    public void OnClick() {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);
        
        float stopedAnimTime = myAnimatorClip[0].clip.length * animState.normalizedTime;
        animator.enabled = false;

        failObj.SetActive(false);
        successObj.SetActive(false);

        successObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "great", false);
        failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);

        if (stopedAnimTime > 1.03f) {
            isSuccess = false;
        }
        else if (stopedAnimTime <= 1.03f && stopedAnimTime > 0.45f) {
            isSuccess = true;
        }
        else {
            isSuccess = false;
        }
        if (isSuccess) {
            successObj.SetActive(true);
            successCnt++;
        }
        else {
            failObj.SetActive(true);
        }

        QTEResults.GetComponent<RectTransform>().localPosition = rect.localPosition;
        nextQTE();
    }

    public void fail() {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);

        float stopedAnimTime = myAnimatorClip[0].clip.length * animState.normalizedTime;
        animator.enabled = false;
        
        successObj.SetActive(false);

        successObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "great", false);
        failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);

        failObj.SetActive(true);

        QTEResults.GetComponent<RectTransform>().localPosition = rect.localPosition;
        nextQTE();
    }

    private void nextQTE() {
        qteCnt++;

        float randX = Random.Range(-630, 630);
        float randY = Random.Range(-300, 300);

        rect.localPosition = new Vector3(randX, randY, 0);
        animator.Play("QTE", -1, 0);
        animator.enabled = true;

        if (qteCnt < 3) {
            sm.qte_magnification = successCnt * 0.1f;
        }
        else {
            _eventManager.TriggerEvent(new SkiJump_QTE_end());
            rect.gameObject.SetActive(false);

            qteCnt = 0;
        }

        transform.parent.GetComponent<Button>().enabled = true;
    }
}
