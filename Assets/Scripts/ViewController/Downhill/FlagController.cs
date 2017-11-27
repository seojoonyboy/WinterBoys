using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public enum type { LEFT, RIGHT };
    public type rayDir;
    public float distance;

    private bool isSend = false;
    private bool isPassSend = false;

    private DownhillManager dm;
    private BoardManager bm;
    void Start() {
        gameObject.tag = "Flag";
    }

    private void Awake() {
        dm = GameObject.Find("Manager").GetComponent<DownhillManager>();
        bm = GameObject.Find("BoardHolder").GetComponent<BoardManager>();
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
                if(dm.playerController.playerState == Downhill_itemType.BOOST) {
                    return;
                }

                if (!isSend) {
                    dm.remainTime -= (int)GameManager.Instance.panelty_time;
                    if(bm.flags.Count != 0) {
                        bm.flags.RemoveAt(0);
                    }
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
                        dm.passNumInc();
                        if (bm.flags.Count != 0) {
                            bm.flags.RemoveAt(0);
                        }
                    }
                    isPassSend = true;
                }
            }
        }

    }
}
