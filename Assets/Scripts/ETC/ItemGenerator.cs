using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemGenerator : MonoBehaviour {
    public GameObject[] items;
    
    public SportType gameType;
    public Transform parent;

    private float interval;
    private Camera cam;

    public float maxTime = 10f;
    public float minTime = 5f;
    private float time;
    private float spawnTime;

    private int[] randNums = { 0, 3, 4, 5 };
    public SkiJumpPlayerController sj_playerController;
    public Ski_PlayerController dh_playerController;

    public int dh_coolTime;
    public int dh_minTime;
    public int dh_maxTime;

    public int sj_coolTime;
    public int sj_minTime;
    public int sj_maxTime;

    private void Start() {
        if(gameType == SportType.SKIJUMP) {
            interval = 300;
            cam = Camera.main;

            time = minTime;
        }
        else if(gameType == SportType.DOWNHILL) {
            interval = -5;
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
        else if(gameType == SportType.DOWNHILL) {
            //Debug.Log(camPos.y);
            if (dh_playerController.playerPos.y < interval + 2) {
                //Debug.Log("아이템 생성");
                Generate(SportType.DOWNHILL);
                interval *= 2;
            }
        }
    }

    private void FixedUpdate() {
        if(gameType == SportType.SKIJUMP) {
            time += Time.deltaTime;
            if (time >= spawnTime) {
                makeBird();
                SetRandomTime();
            }
        }
    }

    //새 이외의 고정되어있는 아이템
    public void Generate(SportType type) {
        int randNum = 0;
        switch (type) {
            case SportType.SKIJUMP:
                Vector3 itemPos = randPos(SportType.SKIJUMP);
                randNum = randNums.Random();
                GameObject item = Instantiate(items[0]);
                item.GetComponent<ItemType>().type = (itemType)randNum;
                item.transform.position = itemPos;
                item.transform.SetParent(parent);
                break;
            case SportType.DOWNHILL:
                Vector3 downhillItemPos = randPos(SportType.DOWNHILL);
                randNum = UnityEngine.Random.Range(1, items.Length);
                GameObject downhillItem = Instantiate(items[randNum]);
                downhillItem.GetComponent<Downhill_ItemType>().type = (Downhill_itemType)randNum;
                downhillItem.transform.SetParent(parent);
                downhillItem.transform.position = downhillItemPos;
                Debug.Log("아이템 생성");
                break;
        }
    }

    private Vector3 randPos(SportType type) {
        Vector3 pos = Vector3.zero;
        float randX = 0;
        float randY = 0;
        switch (type) {
            case SportType.SKIJUMP:
                randX = UnityEngine.Random.Range(0, Screen.width);
                randY = UnityEngine.Random.Range(0, Screen.height - sj_playerController.MaxHeight);

                pos = cam.ScreenToWorldPoint(new Vector3(randX + Screen.width, randY, 0));
                pos.z = 0;
                break;
            case SportType.DOWNHILL:
                randX = UnityEngine.Random.Range(60f, Screen.width - 60f);
                randY = UnityEngine.Random.Range(0, Screen.height);

                pos = cam.ScreenToWorldPoint(new Vector3(randX + Screen.width, dh_playerController.playerPos.y - Screen.height, 3.5f));
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
        spawnTime = UnityEngine.Random.Range(minTime, maxTime);
    }
}
