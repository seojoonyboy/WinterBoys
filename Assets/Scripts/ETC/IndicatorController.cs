using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour {
    public Transform target;
    public BoardManager bM;

    private void Update() {
        if(bM.centers.Count != 0) {
            target = bM.centers[0].transform;

            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, -angle);

            float dist = Vector2.Distance(target.position, transform.position);
            if (target.position.y - transform.position.y > 0.5f || dist < 1.0f) {
                bM.centers.RemoveAt(0);
            }
            //Debug.Log(dist);
        }
    }
}
