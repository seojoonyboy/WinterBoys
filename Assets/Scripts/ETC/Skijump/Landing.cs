using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class Landing : MonoBehaviour {
    private EventManager _eventManger;

    private void Awake() {
        _eventManger = EventManager.Instance;
        _eventManger.AddListener<SkiJump_Resume>(resume);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //Debug.Log("On Coll Enter");
        if (collision.collider.GetType() == typeof(BoxCollider2D)) {
            _eventManger.TriggerEvent(new SkiJump_UnstableLandingEvent());
        }
        else if (collision.collider.GetType() == typeof(CapsuleCollider2D)) {
            //불안정 착지
            
        }

        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "sj_landing");
        _eventManger.TriggerEvent(new SkiJump_LandingEvent());
    }

    private void resume(SkiJump_Resume e) {

    }
}
