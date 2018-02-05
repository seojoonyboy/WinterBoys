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
        failObj,
        realButton;

    private void Awake() {
        animator = GetComponent<Animator>();

        _eventManager = EventManager.Instance;
    }

    private void OnEnable() {
        rect = transform.parent.GetComponent<RectTransform>();
        blur.SetActive(true);
        QTEResults.SetActive(true);
    }

    private void OnDisable() {
        blur.SetActive(false);
    }

    public void OnClick() {
        realButton.GetComponent<Button>().enabled = false;
        StartCoroutine(nextQTE());

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);
        
        float stopedAnimTime = myAnimatorClip[0].clip.length * animState.normalizedTime;
        animator.enabled = false;

        failObj.SetActive(false);
        successObj.SetActive(false);

        //successObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "great", false);
        //failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);

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
            successObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "great", false);
            successCnt++;
            Debug.Log("성공" + successCnt);
        }
        else {
            failObj.SetActive(true);
            failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);
        }

        QTEResults.GetComponent<RectTransform>().localPosition = rect.localPosition;
    }

    public void fail() {
        StartCoroutine(nextQTE());

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);

        float stopedAnimTime = myAnimatorClip[0].clip.length * animState.normalizedTime;
        animator.enabled = false;
        
        successObj.SetActive(false);

        //successObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "great", false);
        //failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);

        failObj.SetActive(true);
        failObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "stupid", false);

        QTEResults.GetComponent<RectTransform>().localPosition = rect.localPosition;
    }

    private IEnumerator nextQTE() {
        yield return sm.WaitForRealSeconds(1.0f);

        realButton.GetComponent<Button>().enabled = true;
        qteCnt++;

        float randX = Random.Range(-630, 630);
        float randY = Random.Range(-300, 300);

        rect.localPosition = new Vector3(randX, randY, 0);
        animator.Play("QTE", -1, 0);
        animator.enabled = true;

        if (qteCnt >= 3) {
            _eventManager.TriggerEvent(new SkiJump_QTE_end());
            rect.gameObject.SetActive(false);
            QTEResults.SetActive(false);

            sm.qte_magnification = successCnt * 0.1f;
            qteCnt = 0;
            successCnt = 0;
        }
    }
}
