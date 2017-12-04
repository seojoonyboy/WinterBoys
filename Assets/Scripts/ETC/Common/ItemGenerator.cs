﻿using System.Collections;
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

    public SkiJumpPlayerController sj_playerController;
    public Ski_PlayerController dh_playerController;

    public int dh_coolTime;
    public int dh_minTime;
    public int dh_maxTime;

    public int sj_coolTime;
    public int sj_minTime;
    public int sj_maxTime;

    private GameManager gm;

    private GameManager.percentages dh_percentages;
    private GameManager.percentages sj_percentages;
    private GameManager.percentages st_percentages;

    private float[]
        dh_percentagesToArr,
        sj_percentagesToArr,
        st_percentagesToArr;

    private void Start() {
        init();

        if (gameType == SportType.SKIJUMP) {
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
                //makeBird();
                SetRandomTime();
            }
        }
    }

    public void Generate(SportType type) {
        int randNum = UnityEngine.Random.Range(0, 100);
        int itemIndex = getPercentageBasedIndex(type, randNum);
        GameObject item = Instantiate(items[itemIndex]);

        switch (type) {
            case SportType.SKIJUMP:
                item.GetComponent<Item>().item_sj = (ItemType.SJ)itemIndex;
                break;
            case SportType.DOWNHILL:
                item.GetComponent<Item>().item_dh = (ItemType.DH)itemIndex;
                break;
            case SportType.SKELETON:
                item.GetComponent<Item>().item_st = (ItemType.ST)itemIndex;
                break;
        }

        Vector3 itemPos = randPos(type);
        item.GetComponent<Item>().gameType = type;
        item.transform.position = itemPos;
        item.transform.SetParent(parent);
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

    //아이템 드롭 확률 기반 아이템 Index값
    private int getPercentageBasedIndex(SportType type, int randNum) {
        int itemIndex = 0;
        switch (type) {
            case SportType.DOWNHILL:
                for (int i = 0; i < dh_percentagesToArr.Length - 1; i++) {
                    if (randNum < dh_percentagesToArr[i]) {
                        itemIndex = i;
                    }
                }
                break;

            case SportType.SKIJUMP:
                for (int i = 0; i < sj_percentagesToArr.Length - 1; i++) {
                    if (randNum < sj_percentagesToArr[i]) {
                        itemIndex = i;
                    }
                }
                break;
            case SportType.SKELETON:
                for (int i = 0; i < st_percentagesToArr.Length - 1; i++) {
                    if (randNum < st_percentagesToArr[i]) {
                        itemIndex = i;
                    }
                }
                break;
        }
        return itemIndex;
    }

    //private void makeBird() {
    //    time = 0;
    //    int[] randNums = { 1, 2 };
    //    int randNum = randNums.Random();
    //    GameObject obj = Instantiate(items[randNum]);
    //    obj.transform.position = randPos(SportType.SKIJUMP);
    //}

    private void SetRandomTime() {
        spawnTime = UnityEngine.Random.Range(minTime, maxTime);
    }

    private void init() {
        gm = GameManager.Instance;

        dh_percentages = gm.dh_percentages;
        sj_percentages = gm.sj_percentages;
        st_percentages = gm.st_percentages;

        dh_percentagesToArr = new float[dh_percentages.values.Length];
        sj_percentagesToArr = new float[sj_percentages.values.Length];
        st_percentagesToArr = new float[st_percentages.values.Length];

        float sum = 0;
        for(int i=0; i<dh_percentages.values.Length; i++) {
            sum += dh_percentages.values[i];
            dh_percentagesToArr[i] = sum;
        }

        sum = 0;
        for (int i = 0; i<sj_percentages.values.Length; i++) {
            sum += sj_percentages.values[i];
            sj_percentagesToArr[i] = sum;
        }

        sum = 0;
        for (int i = 0; i < st_percentages.values.Length; i++) {
            sum += st_percentages.values[i];
            st_percentagesToArr[i] = sum;
        }
    }
}