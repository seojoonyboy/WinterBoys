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
        successObj,
        failObj;

    private void Awake() {
        animator = GetComponent<Animator>();

        _eventManager = EventManager.Instance;
    }

    private void OnEnable() {
        rect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnClick() {
        successObj.SetActive(false);
        failObj.SetActive(false);

        successObj.GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();
        failObj.GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(0);
        float stopedAnimTime = myAnimatorClip[0].clip.length * animState.normalizedTime;
        animator.speed = 0.0f;

        if (stopedAnimTime > 0.9999f) {
            isSuccess = false;
        }
        else if (stopedAnimTime < 0.9999f && stopedAnimTime > 0.45f) {
            isSuccess = true;
        }
        else {
            isSuccess = false;
        }
        nextQTE();

        Vector3 newPosOfResultText = new Vector3(rect.transform.Find("QTE").position.x, rect.transform.Find("QTE").position.y + 230f, 0);
        if (isSuccess) {
            successObj.GetComponent<RectTransform>().localPosition = newPosOfResultText;
            successObj.SetActive(true);
        }
        else {
            failObj.GetComponent<RectTransform>().localPosition = newPosOfResultText;
            failObj.SetActive(true);
        }
    }

    private void nextQTE() {
        qteCnt++;

        float randX = Random.Range(-630, 630);
        float randY = Random.Range(-300, 300);

        rect.localPosition = new Vector3(randX, randY, 0);

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("QTE", -1, 0);
        animator.speed = 1.0f;

        if (qteCnt < 3) {
            sm.qte_magnification = successCnt * 0.1f;
        }
        else {
            _eventManager.TriggerEvent(new SkiJump_QTE_end());
            rect.gameObject.SetActive(false);

            qteCnt = 0;
        }
    }
}
