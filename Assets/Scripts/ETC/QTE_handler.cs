using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

public class QTE_handler : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        EventManager.Instance.TriggerEvent(new SkiJump_QTE_start());
        Destroy(GetComponent<QTE_handler>());
    }
}