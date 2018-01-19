using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagFilter : MonoBehaviour {
    bool isSolved = false;
    bool minXFinded = false;
    bool maxXFinded = false;
    private Poses xPoses;
    
    [SerializeField] private float minimumSpacing = 0;
    private float refinedMinSpacing = 0;

    private float limitTime = 2.0f;
    private bool isAnyTileFounded = false;

    private void Start() {
        float itemWidth = 0;
        if(GetComponent<BoxCollider2D>() != null) {
            itemWidth = (float)(GetComponent<BoxCollider2D>().size.x / 2.0f);
        }
        else {
            if(GetComponent<PolygonCollider2D>() != null) {
                itemWidth = 0.1f;
            }
            itemWidth = 0;
        }

        xPoses = new Poses();
        refinedMinSpacing = (float)(minimumSpacing/GameManager.Instance.pixelPerUnit) + itemWidth;
        InvokeRepeating("check", 0, 0.1f);
    }

    private void FixedUpdate() {
        var item = GetComponent<Item>();
        if(item == null) { return; }

        if (item.item_dh == ItemType.DH.ENEMY_BEAR) {
            AdditionalHandler();
        }
    }

    private void AdditionalHandler() {
        if (isAnyTileFounded) {
            limitTime -= Time.deltaTime;
        }

        if (limitTime < 0) {
            GetComponent<BearMove>().enabled = true;
            Destroy(GetComponent<FlagFilter>());
        }
    }

    private void stopIvoke() {
        CancelInvoke("check");
    }

    private void check() {
        if(minXFinded || maxXFinded) {
            isAnyTileFounded = true;
        }

        if (isSolved) {
            stopIvoke();
            setPoses();
        }

        if(minXFinded && maxXFinded) {
            isSolved = true;
            //Debug.Log("발견된 X 최솟값 : " + xPoses.min + " 발견된 X 최댓값 : " + xPoses.max);
            return;
        }

        Vector2 rightStartPos = new Vector2(transform.position.x + 0.2f, transform.position.y);
        RaycastHit2D rightHit = Physics2D.Raycast(rightStartPos, Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));

        if (rightHit.collider != null) {
            if (rightHit.collider.gameObject.CompareTag("DH_rightTile")) {
                xPoses.max = rightHit.point.x - refinedMinSpacing;

                RaycastHit2D leftHit = Physics2D.Raycast(transform.position, -Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));
                if (leftHit.collider != null) {
                    if(leftHit.collider.gameObject.CompareTag("DH_leftTile")) {
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
            Vector2 leftStartPos = new Vector2(transform.position.x - 0.1f, transform.position.y);
            RaycastHit2D leftHit = Physics2D.Raycast(leftStartPos, -Vector2.right, Mathf.Infinity, 1 << LayerMask.NameToLayer("dh_SideTile"));
            if (leftHit.collider != null) {
                if (leftHit.collider.gameObject.CompareTag("DH_rightTile")) {
                    transform.position = leftHit.point;
                    check();
                }
                else if(leftHit.collider.gameObject.CompareTag("DH_leftTile")) {
                    xPoses.min = leftHit.point.x + refinedMinSpacing;
                    xPoses.max = transform.position.x - refinedMinSpacing;

                    minXFinded = true;
                    maxXFinded = true;
                }
            }
        }
    }

    private void setPoses() {
        float randX = Random.Range(xPoses.min, xPoses.max);
        transform.position = new Vector2(randX, transform.position.y);

        if(GetComponent<Item>() == null) { return; }

        var item = GetComponent<Item>();
        if(item.item_dh == ItemType.DH.ENEMY_BEAR) {
            GetComponent<BearMove>().enabled = true;
        }
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
