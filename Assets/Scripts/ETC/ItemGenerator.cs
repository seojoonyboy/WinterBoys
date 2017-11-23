using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {
    public SportType gameType;
    public GameObject[] items;
    public Transform parent;

    private float interval;
    private Camera cam;

    private float maxTime = 10f;
    private float minTime = 5f;
    private float time;
    private float spawnTime;

    private int[] randNums = { 0, 3, 4, 5 };
    public SkiJumpPlayerController playerController;
    private void Start() {
        if(gameType == SportType.SKIJUMP) {
            interval = 100;
            cam = Camera.main;
        }
        time = minTime;
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

    private void FixedUpdate() {
        time += Time.deltaTime;

        if(time >= spawnTime) {
            makeBird();
            SetRandomTime();
        }
    }

    //새 이외의 고정되어있는 아이템
    public void Generate(SportType type) {
        switch (type) {
            case SportType.SKIJUMP:
                Vector3 itemPos = randPos(SportType.SKIJUMP);
                int randNum = randNums.Random();
                GameObject item = Instantiate(items[0]);
                item.GetComponent<ItemType>().type = (itemType)randNum;
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

    private void makeBird() {
        time = 0;
        int[] randNums = { 1, 2 };
        int randNum = randNums.Random();
        GameObject obj = Instantiate(items[randNum]);
        obj.transform.position = randPos(SportType.SKIJUMP);
    }

    private void SetRandomTime() {
        spawnTime = Random.Range(minTime, maxTime);
    }
}
