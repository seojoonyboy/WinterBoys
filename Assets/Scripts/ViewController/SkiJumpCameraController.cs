using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpCameraController : MonoBehaviour {
    public Transform target;

    private int amount = 0;
    private bool needZoom = false;
    private float startTime = 0;
    private float journeyLength;

    private void Update() {
        if (needZoom) {
            float distCovered = (Time.time - startTime);
            //Debug.Log(distCovered);
            if(distCovered >= 0.08f) {
                needZoom = false;
                Time.timeScale = 1.0f;
            }
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(target.position.x, target.position.y, target.position.z - 10f + amount), fracJourney);
        }
        else {
            //transform.position = new Vector3(target.position.x, target.position.y, target.position.z - 10f);
            transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(target.position.x, target.position.y, target.position.z - 10f), 1);
        }
    }

    public void zoomIn(int amount = 0) {
        journeyLength = Vector3.Distance(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + amount));
        this.amount = amount;
        startTime = Time.time;

        needZoom = true;
    }
}
