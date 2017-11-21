using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgColl : MonoBehaviour {
    public _type type;
    public enum _type { SKIJUMP, DOWNHILL }

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isTriggered) return;

        if (type == _type.SKIJUMP) {
            GameObject.Find("Managers").GetComponent<SkiJumpBoardHolder>().GenerateNextSet();
        }

        isTriggered = true;
    }
}
