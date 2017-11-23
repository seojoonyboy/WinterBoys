using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {
    public SportType gameType;
    public GameObject[] items;
    public Transform parent;

    private float interval;
    private Camera cam;

    public SkiJumpPlayerController playerController;
    private void Start() {
        if(gameType == SportType.SKIJUMP) {
            interval = 100;
            cam = Camera.main;
        }
    }

    private void Update() {
        if(gameType == SportType.SKIJUMP) {
            Vector2 camPos = cam.transform.position;
            if (camPos.x > interval) {
                Generate(SportType.SKIJUMP);
                interval *= 2;
            }
        }
    }

    public void Generate(SportType type) {
        switch (type) {
            case SportType.SKIJUMP:
                Vector3 itemPos = randPos(SportType.SKIJUMP);
                int randIndex = Random.Range(0, items.Length);
                GameObject item = Instantiate(items[randIndex]);
                item.transform.position = itemPos;
                item.transform.SetParent(parent);
                break;
        }
    }

    private Vector3 randPos(SportType type) {
        Vector3 pos = Vector3.zero;
        switch (type) {
            case SportType.SKIJUMP:
                float randX = Random.Range(0, Screen.width);
                float randY = Random.Range(0, Screen.height - playerController.MaxHeight);

                pos = cam.ScreenToWorldPoint(new Vector3(randX + Screen.width, randY, 0));
                pos.z = 0;
                break;
        }
        return pos;
    }
}
