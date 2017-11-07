using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpCameraController : MonoBehaviour {
    public Transform target;

    public delegate void ZoomOutHandler();
    public static event ZoomOutHandler OffZooming;

    private int amount = 0;
    public bool needZoom = false;
    private float startTime = 0;
    private float journeyLength;

    private void OnEnable() {
        ArrowRotate.OnRotatingEnd += endRotating;
    }

    private void OnDisable() {
        ArrowRotate.OnRotatingEnd -= endRotating;
    }

    private void Start() {
        journeyLength = Vector3.Distance(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + amount));
    }

    private void Update() {
        if (needZoom) {
            float distCovered = (Time.time - startTime);

            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(target.position.x, target.position.y, target.position.z - 10f + amount), fracJourney);
        }
        else {
            transform.position = new Vector3(target.position.x, target.position.y, target.position.z - 10f);
        }
    }

    public void zoomIn(int amount = 0) {
        this.amount = amount;
        startTime = Time.time;

        needZoom = true;
    }

    public void zoomOut() {
        OffZooming();
        needZoom = false;
        transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(target.position.x, target.position.y, target.position.z - 10f), 1);
    }

    void endRotating() {
        zoomOut();
    }
}
