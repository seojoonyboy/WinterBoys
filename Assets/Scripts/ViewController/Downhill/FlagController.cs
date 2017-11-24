using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public enum type { LEFT, RIGHT };
    public type rayDir;
    public float distance;

    private bool isSend = false;
    private bool isPassSend = false;
    void Start() {
        gameObject.tag = "Flag";
    }

    private void FixedUpdate() {
        IsFail();
        IsPass();
    }

    void IsFail() {
        Vector3 dir = Vector3.zero;
        if (rayDir == type.LEFT) {
            dir = Vector3.left;
        }
        else if (rayDir == type.RIGHT) {
            dir = Vector3.right;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit)) {
            if (hit.collider.tag == "Player") {
                if(GameObject.Find("Manager").GetComponent<DownhillManager>().playerController.playerState == Downhill_itemType.BOOST) {
                    return;
                }

                if (!isSend) {
                    GameObject.Find("Manager").GetComponent<DownhillManager>().remainTime -= (int)GameManager.Instance.panelty_time;
                }
                isSend = true;
            }
        }
    }
    void IsPass() {
        RaycastHit hit;
        Vector3 dir = Vector3.zero;
        if (rayDir == type.LEFT) {
            dir = Vector3.right;

            if (Physics.Raycast(transform.position, dir, out hit, distance)) {
                if (hit.collider.tag == "Player") {
                    if (!isPassSend) {
                        //Debug.Log(hit.collider.tag + " 통과");
                        GameObject.Find("Manager").GetComponent<DownhillManager>().passNumInc();
                    }
                    isPassSend = true;
                }
            }
        }

    }
}
