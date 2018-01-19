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
    private bool canGenerate = true;

    public SkiJumpPlayerController sj_playerController;
    public Ski_PlayerController dh_playerController;

    //다운힐
    public int[] dh_standardChangeMeter;  //아이템 등장 기준 변경 시점 (미터)
    public int[] dh_intervalMeter;        //등장 간격 (미터)
    public int[] dh_itemArea;             //등장 공간
    public int[] dh_numPerGenerate;       //등장 갯수
    private int dh_index;
    private int dh_interval;

    //스키점프
    public int[] sj_standardChangeMeter;  //아이템 등장 기준 변경 시점 (미터)
    public int[] sj_intervalMeter;        //등장 간격 (미터)
    public int[] sj_numPerGenerate;       //등장 갯수
    private int sj_index;
    private int sj_interval;

    //스켈레톤
    public int[] st_standardChangeMeter;    //아이템 등장 기준 변경 시점 (미터)
    public int[] st_intervalMeter;          //등장 간격 (미터)

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
            cam = Camera.main;
            time = minTime;
        }
        else if(gameType == SportType.DOWNHILL) {
            cam = Camera.main;
        }
    }

    private void Update() {
        if(gameType == SportType.SKIJUMP) {
            if (!canGenerate) { return; }
            float charPosOfX = sj_playerController.rb.transform.position.x;

            if (charPosOfX >= sj_standardChangeMeter[sj_index]) {
                if(sj_index == sj_standardChangeMeter[sj_index]) { return; }

                for(int i=0; i<sj_numPerGenerate[sj_index]; i++) {
                    Generate(SportType.SKIJUMP, sj_numPerGenerate[sj_index]);
                }

                sj_interval = sj_standardChangeMeter[sj_index] + sj_intervalMeter[sj_index];

                if(sj_index == sj_standardChangeMeter.Length - 1) {
                    canGenerate = false;
                    return;
                }

                sj_index++;
            }

            if(sj_index != 0) {
                if (charPosOfX > sj_interval) {
                    for(int i=0; i<sj_numPerGenerate[sj_index]; i++) {
                        Generate(SportType.SKIJUMP, sj_numPerGenerate[sj_index - 1]);
                    }
                    sj_interval += sj_intervalMeter[sj_index];
                }
            }
        }
        else if(gameType == SportType.DOWNHILL) {
            float charPosOfY = dh_playerController.virtualPlayerPosOfY;
            //다음 아이템 젠 변경까지
            if (dh_index != 0) {
                if (charPosOfY > dh_interval) {
                    for (int i = 0; i < dh_numPerGenerate[dh_index]; i++) {
                        Generate(SportType.DOWNHILL, dh_numPerGenerate[dh_index - 1]);
                    }
                    dh_interval += dh_intervalMeter[dh_index];
                }
            }

            if (!canGenerate) { return; }
            //아이템 젠 변경점
            if (charPosOfY > dh_standardChangeMeter[dh_index]) {
                for (int i=0; i<dh_numPerGenerate[dh_index]; i++){
                    Generate(SportType.DOWNHILL, dh_numPerGenerate[dh_index]);
                }

                dh_interval = dh_standardChangeMeter[dh_index] + dh_intervalMeter[dh_index];

                if (dh_index == dh_standardChangeMeter.Length - 1) {
                    canGenerate = false;
                    return;
                }
                dh_index++;
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

    public void Generate(SportType type, int numPerGen = 1) {
        int randNum = UnityEngine.Random.Range(0, 100);
        int itemIndex = getPercentageBasedIndex(type, randNum);
        GameObject item = Instantiate(items[itemIndex]);

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
                randY = UnityEngine.Random.Range(4, 33);
                pos = new Vector2(
                    sj_playerController.transform.position.x + randX + 120f,
                    randY
                );
                break;
            case SportType.DOWNHILL:
                randX = UnityEngine.Random.Range(0, Screen.width);
                float randInterval = UnityEngine.Random.Range(0, (float)(dh_itemArea[dh_index]/gm.pixelPerUnit));
                pos = new Vector2(
                    dh_playerController.playerPos.x,
                    dh_playerController.playerPos.y - (float)(Screen.height / gm.pixelPerUnit) * 2f - randInterval
                );
                break;
        }
        return pos;
    }

    //아이템 드롭 확률 기반 아이템 Index값
    private int getPercentageBasedIndex(SportType type, int randNum) {
        int itemIndex = 0;
        switch (type) {
            case SportType.DOWNHILL:
                for (int i = 0; i < dh_percentagesToArr.Length; i++) {
                    if (randNum < dh_percentagesToArr[i]) {
                        itemIndex = i;
                        break;
                    }
                }
                break;

            case SportType.SKIJUMP:
                for (int i = 0; i < sj_percentagesToArr.Length; i++) {
                    if (randNum < sj_percentagesToArr[i]) {
                        itemIndex = i;
                        break;
                    }
                }
                break;
            case SportType.SKELETON:
                for (int i = 0; i < st_percentagesToArr.Length; i++) {
                    if (randNum < st_percentagesToArr[i]) {
                        itemIndex = i;
                        break;
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

        if(gameType == SportType.DOWNHILL) {
            dh_index = 0;
            dh_interval = dh_intervalMeter[0];
        }
        else if(gameType == SportType.SKIJUMP) {
            sj_index = 0;
            sj_interval = sj_intervalMeter[0];
        }
    }
}
