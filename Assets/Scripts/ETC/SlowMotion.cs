using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour {
    public SkiJumpManager sm;
    public SkiJumpCameraController cameraController;

    private void OnCollisionEnter2D(Collision2D collision) {
        Time.timeScale = sm.slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;

        cameraController.zoomIn(5);
    }
}
