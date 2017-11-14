using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour {
    private EventManager _eventManger;
    public SkiJumpManager sm;
    
    private bool isFirst = true;

    private void Awake() {
        _eventManger = EventManager.Instance;
        _eventManger.AddListener<SkiJumpEvent>(OnJump);
    }

    private void OnJump(SkiJumpEvent e) {
        Time.timeScale = sm.slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;

        isFirst = false;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (isFirst) {
            _eventManger.TriggerEvent(new SkiJumpEvent());
        }
    }
}
