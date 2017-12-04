using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class SlowMotion : MonoBehaviour {
    private EventManager _eventManger;
    public SkiJumpManager sm;
    
    private bool isFirst = true;

    private void Awake() {
        _eventManger = EventManager.Instance;
        _eventManger.AddListener<SkiJump_JumpEvent>(OnJump);
    }

    private void OnJump(SkiJump_JumpEvent e) {
        Time.timeScale = sm.slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;

        isFirst = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isFirst) {
            _eventManger.TriggerEvent(new SkiJump_JumpEvent());
        }
    }
}
