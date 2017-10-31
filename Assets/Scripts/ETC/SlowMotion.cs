using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour {
    public SkiJumpManager sm;

    public delegate void EnterColliderHandler();
    public static event EnterColliderHandler OnJumpArea;

    public delegate void ExitColliderHandler();
    public static event ExitColliderHandler ExitJumpArea;
    private void OnCollisionEnter2D(Collision2D collision) {
        Time.timeScale = sm.slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;

        OnJumpArea();
    }

    private void OnCollisionExit2D(Collision2D collision) {
        //Debug.Log("충돌 감지 영역 벗어남");
        ExitJumpArea();
    }
}
