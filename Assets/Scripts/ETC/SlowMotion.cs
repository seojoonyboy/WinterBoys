using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour {
    public SkiJumpManager sm;

    public delegate void EnterColliderHandler();
    public static event EnterColliderHandler OnJumpArea;
    
    private bool isFirst = true;

    private void OnCollisionExit2D(Collision2D collision) {
        if (isFirst) {
            Time.timeScale = sm.slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;

            OnJumpArea();

            isFirst = false;
        }
    }
}
