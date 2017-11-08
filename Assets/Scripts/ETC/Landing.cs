using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour {
    public delegate void EnterColliderHandler();
    public delegate void UnstableLandHandler();
    
    public static event EnterColliderHandler OnLanding;
    public static event UnstableLandHandler UnstableLanding;

    private bool isFirst;

    private void OnEnable() {
        isFirst = true;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.GetType() == typeof(BoxCollider2D)) {
            //Debug.Log("캐릭터 지면 접촉");
            UnstableLanding();
        }
        else if (collision.collider.GetType() == typeof(CapsuleCollider2D)) {
            //Debug.Log("플레이트 지면 접촉");
        }

        if (isFirst) {
            OnLanding();
            isFirst = false;
        }
    }
}
