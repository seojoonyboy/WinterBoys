using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class Landing : MonoBehaviour {
    private EventManager _eventManger;

    private bool isFirst;

    private void Awake() {
        _eventManger = EventManager.Instance;
    }

    private void OnEnable() {
        isFirst = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.GetType() == typeof(BoxCollider2D)) {
            Debug.Log("캐릭터 지면 접촉");
            _eventManger.TriggerEvent(new SkiJump_UnstableLandingEvent());
        }
        else if (collision.collider.GetType() == typeof(CapsuleCollider2D)) {
            //Debug.Log("플레이트 지면 접촉");
        }

        if (isFirst) {
            _eventManger.TriggerEvent(new SkiJump_LandingEvent());
            isFirst = false;
        }
    }
}
