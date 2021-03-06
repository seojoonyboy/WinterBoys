﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class CharResumePosFilter : MonoBehaviour {
    bool isSolved = false;
    bool minXFinded = false;
    bool maxXFinded = false;
    private Poses xPoses;

    RaycastHit2D rightHit;
    RaycastHit2D leftHit;

    [SerializeField] private float minimumSpacing = 0;
    public float limitedTime = 2.0f;

    private float refinedMinSpacing = 0;

    private void OnEnable() {
        isSolved = false;
        minXFinded = false;
        maxXFinded = false;

        InvokeRepeating("check", 0, 0.1f);
    }

    private void Start() {
        xPoses = new Poses();
        refinedMinSpacing = (float)(GameManager.Instance.pixelPerUnit / minimumSpacing);
    }

    private void Update() {
        limitedTime -= Time.deltaTime;

        if (limitedTime < 0) {
            Debug.Log("경로 재탐색");
            if(rightHit.collider != null) {
                xPoses.min = rightHit.point.x;
                xPoses.max = rightHit.point.x;
            }
            else {
                if (leftHit.collider != null) {
                    xPoses.min = rightHit.point.x;
                    xPoses.max = rightHit.point.x;
                }
            }
            minXFinded = true;
            maxXFinded = true;
            isSolved = true;

            limitedTime = 2.0f;
        }
    }

    private void stopIvoke() {
        CancelInvoke("check");
    }

    private void check() {
        if (isSolved) {
            stopIvoke();
            setPoses();
        }

        if (minXFinded && maxXFinded) {
            isSolved = true;
            //Debug.Log("발견된 X 최솟값 : " + xPoses.min + " 발견된 X 최댓값 : " + xPoses.max);
            return;
        }

        Vector2 rightStartPos = new Vector2(transform.position.x + 0.2f, transform.position.y);
        rightHit = Physics2D.Raycast(rightStartPos, Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));

        if (rightHit.collider != null) {
            //Debug.Log("오른쪽 충돌체 : " + rightHit.collider.name);
            if (rightHit.collider.gameObject.CompareTag("DH_rightTile")) {
                xPoses.max = rightHit.point.x - refinedMinSpacing;

                Vector2 leftStartPos = new Vector2(transform.position.x - 0.2f, transform.position.y);
                leftHit = Physics2D.Raycast(leftStartPos, Vector2.left, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));
                if (leftHit.collider != null) {
                    //Debug.Log("왼쪽 충돌체 : " + leftHit.collider.name);
                    if (leftHit.collider.gameObject.CompareTag("DH_leftTile")) {
                        xPoses.min = leftHit.point.x + refinedMinSpacing;
                    }
                }
                else {
                    xPoses.min = transform.position.x + refinedMinSpacing;
                }

                minXFinded = true;
                maxXFinded = true;
            }
            else if (rightHit.collider.gameObject.CompareTag("DH_leftTile")) {
                transform.position = rightHit.point;
                check();
            }
        }
        else {
            Vector2 leftStartPos = new Vector2(transform.position.x - 0.2f, transform.position.y);
            leftHit = Physics2D.Raycast(leftStartPos, Vector2.left, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));
            if (leftHit.collider != null) {
                //Debug.Log("왼쪽 충돌체 : " + leftHit.collider.name);
                if (leftHit.collider.gameObject.CompareTag("DH_rightTile")) {
                    transform.position = leftHit.point;
                    check();
                }
                else if (leftHit.collider.gameObject.CompareTag("DH_leftTile")) {
                    xPoses.min = leftHit.point.x + refinedMinSpacing;
                    xPoses.max = transform.position.x - refinedMinSpacing;

                    minXFinded = true;
                    maxXFinded = true;
                }
            }
        }
    }

    private void setPoses() {
        float middle = (xPoses.min + xPoses.max) / 2.0f;
        transform.position = new Vector3(middle, transform.position.y, -0.2f);
        enabled = false;

        EventManager.Instance.TriggerEvent(new Downhill_RepositionCharToResumeFinished());
    }

    class Poses {
        public float min;
        public float max;

        public Poses() {
            min = 0;
            max = 0;
        }
    }
}
