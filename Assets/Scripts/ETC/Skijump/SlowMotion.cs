using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class SlowMotion : MonoBehaviour {
    private EventManager _eventManger;
    public SkiJumpManager sm;
    
    private bool isFirst = true;
    public AudioSource extraAudioSource;

    private void Awake() {
        _eventManger = EventManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(isFirst && collision.tag == "Player") {
            _eventManger.TriggerEvent(new SkiJump_JumpEvent());
            isFirst = false;

            Time.timeScale = sm.slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;

            extraAudioSource.clip = SoundManager.Instance.scene_sj_effects[3];
            extraAudioSource.Play();
        }
    }
}
