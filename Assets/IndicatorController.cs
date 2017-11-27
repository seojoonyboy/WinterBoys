using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour {
    public Transform target;
    public BoardManager bM;

    private void Update() {
        target = bM.flags[0].transform;
        Vector3 tmp = new Vector3(target.position.x + bM.deltaX, target.position.y);
        Vector2 direction = tmp - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, -angle);

        float dist = Vector2.Distance(target.position, transform.position);
        if(dist < 1) {
            bM.flags.RemoveAt(0);
        }
    }
}
