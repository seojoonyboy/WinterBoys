using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour {
    public enum type { LEFT, RIGHT };
    public type rayDir;

    void Start() {
        InvokeRepeating("IsInArea", 1.0f, 1.0f);
    }

    private void FixedUpdate() {
        IsFail();
    }

    void IsInArea() {
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);

        if(pos.y >= 1.0f) {
            Destroy(gameObject);
        }
    }

    void IsFail() {
        RaycastHit hit;
        
        //Debug.DrawRay(transform.position, Vector3.left, Color.red);
        Vector3 dir = Vector3.zero;
        if(rayDir == type.LEFT) {
            dir = Vector3.left;
        }
        else if(rayDir == type.RIGHT) {
            dir = Vector3.right;
        }

        if(Physics.Raycast(transform.position, dir, out hit)) {
            if(hit.collider.tag == "Player")  {
                //game over
                GameManager.Instance.gameOver();
                GameObject.Find("Manager").GetComponent<DownhillManager>().modal.SetActive(true);
            }
        }
    }
}
