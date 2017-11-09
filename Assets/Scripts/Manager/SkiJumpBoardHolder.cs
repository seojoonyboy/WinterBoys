using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpBoardHolder : MonoBehaviour {
    public GameObject[] 
        mountainPrefs,
        cloudPrefs;
    
    //생성된 프리팹간 간격
    public float 
        intervalsOfMountains,
        intervalOfClouds;

    //한번 생성시 몇개씩 생성할 것인지.
    public int
        bundleOfMountains = 1,
        bundleOfClouds = 1;

    //한번에 2개 이상 생성할 때 서로간 상대적 무작위 간격(상하좌우)을 위한 상수
    public int[] 
        offSetOfMountain,
        offSetOfCloud;

    private int mountainIndex = 0;
    private int cloudIndex = 0;

    public Transform holder;
    private Vector2 lastPrefPos;
    private void Awake() {
        
    }

    private void Start() {
        init();
    }

    private void Update() {
        float camXPos = Camera.main.transform.position.x;
    }

    private void init() {
        mountainIndex = 0;
        cloudIndex = 0;

        for(int i=0; i<=30; i++) {
            Generate(bundleOfMountains, 0);
            Generate(bundleOfClouds, 1);
        }
    }

    private Vector2 randPoses(Vector2 origin, int type) {
        Vector2 nextPos;
        float 
            xPos = 0,
            yPos = 0;
        switch (type) {
            case 0:
                xPos = origin.x + randNum(offSetOfMountain[0], offSetOfMountain[1]);
                yPos = origin.y + randNum(offSetOfMountain[2], offSetOfMountain[3]);
                break;
            case 1:
                xPos = origin.x + randNum(offSetOfCloud[0], offSetOfCloud[1]);
                yPos = origin.y + randNum(offSetOfCloud[2], offSetOfCloud[3]);
                break;
        }
        nextPos = new Vector2(xPos, yPos);
        return nextPos;
    }

    private float randNum(int min, int max) {
        int[] marks = { -1, 1 };
        int mark = marks.Random();
        float rand = Random.Range(min, max);
        return rand * mark;
    }

    private void Generate(int num, int type) {
        switch (type) {
            //산악지형
            case 0:
                int mntRndIndex = Random.Range(0, mountainPrefs.Length - 1);
                Vector2 originPos = new Vector2(intervalsOfMountains * mountainIndex, 0);

                float totalMountainNum = Random.Range(1, num);
                if (totalMountainNum > 1) {
                    for (int i = 0; i < num - 1; i++) {
                        mntRndIndex = Random.Range(0, mountainPrefs.Length);
                        //Debug.Log(mntRndIndex);
                        GameObject obj = Instantiate(mountainPrefs[mntRndIndex]);
                        obj.transform.position = randPoses(originPos, 0);
                        obj.transform.SetParent(holder, false);
                    }
                }
                break;
            //구름
            case 1:
                int cloudRndIndex = Random.Range(0, cloudPrefs.Length - 1);
                Vector2 cloudOriginPos = new Vector2(intervalOfClouds * cloudIndex, 100);
                float totalCloudNum = Random.Range(1, num);
                if(totalCloudNum > 1) {
                    for (int i = 0; i < num - 1; i++) {
                        cloudRndIndex = Random.Range(0, cloudPrefs.Length);
                        GameObject _obj = Instantiate(cloudPrefs[cloudRndIndex]);
                        _obj.transform.position = randPoses(cloudOriginPos, 1);
                        _obj.transform.SetParent(holder, false);
                    }
                }
                break;
        }
        cloudIndex++;
        mountainIndex++;
    }
}
