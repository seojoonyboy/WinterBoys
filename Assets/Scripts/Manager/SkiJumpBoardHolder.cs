using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpBoardHolder : MonoBehaviour {
    public GameObject[] 
        mountainPrefs,
        cloudPrefs,
        starPrefs;

    public GameObject 
        skyPref,
        groundPref;

    //생성된 프리팹간 간격
    public float intervalOfClouds;

    //한번 생성시 몇개씩 생성할 것인지.
    public int bundleOfClouds = 1;

    //다음 객체 상대적 위치
    public int[] 
        offSetOfMountain,
        offSetOfCloud,
        offsetOfStar;

    private int mountainIndex = 0;
    private int cloudIndex = 0;

    public Transform holder;
    private Vector2 
        lastMountainPos,
        nextSetPos,
        nextStarPos,
        cloudStartPos;

    private void Awake() {
        
    }

    private void Start() {
        init();
    }

    private void Update() {
        float camXPos = Camera.main.transform.position.x;
    }

    private void init() {
        lastMountainPos = new Vector2(-20, 0);
        nextSetPos = new Vector2(120, 0);
        nextStarPos = new Vector2(-16, 28);
        cloudStartPos = new Vector2(-11, 15);

        mountainIndex = 0;
        cloudIndex = 0;

        Generate(10, 0);
        Generate(20, 1);
        Generate(20, 2);

        for (int i=0; i<=30; i++) {
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
            case 2:
                xPos = origin.x + randNum(offsetOfStar[0], offsetOfStar[1]);
                yPos = randNum(offsetOfStar[2], offsetOfStar[3]);
                break;
        }
        nextPos = new Vector2(xPos, yPos);
        return nextPos;
    }

    private float randNum(int min, int max, bool needMark = false) {
        float rand = Random.Range(min, max);
        float val = 0;
        if (needMark) {
            int[] marks = { -1, 1 };
            int mark = marks.Random();
            val = rand * mark;
        }
        else {
            val = rand;
        }
        return val;
    }

    private void Generate(int num, int type) {
        switch (type) {
            //산악지형
            case 0:
                for(int i=0; i<num; i++) {
                    Vector2 lastPos = new Vector2(lastMountainPos.x, 0);
                    int imageIndex = Random.Range(0, mountainPrefs.Length);
                    GameObject mountainObj = Instantiate(mountainPrefs[imageIndex]);
                    mountainObj.transform.position = randPoses(lastPos, 0);
                    mountainObj.transform.SetParent(holder, false);

                    lastMountainPos = mountainObj.transform.position;
                }
                break;
            //구름
            case 1:
                int cloudRndIndex = Random.Range(0, cloudPrefs.Length - 1);
                Vector2 cloudOriginPos = new Vector2(cloudStartPos.x + intervalOfClouds * cloudIndex, cloudStartPos.y);
                float totalCloudNum = Random.Range(15, num);
                if(totalCloudNum > 1) {
                    for (int i = 0; i < num - 1; i++) {
                        cloudRndIndex = Random.Range(0, cloudPrefs.Length);
                        GameObject _obj = Instantiate(cloudPrefs[cloudRndIndex]);
                        _obj.transform.position = randPoses(cloudOriginPos, 1);
                        _obj.transform.SetParent(holder, false);
                    }
                }
                cloudIndex++;
                break;
            //별
            case 2:
                int starImgIndx = Random.Range(0, starPrefs.Length);
                Vector2 otherLayerPos = new Vector2(nextSetPos.x, 25);
                for (int i = 0; i < num; i++) {
                    GameObject starObj = Instantiate(starPrefs[starImgIndx]);
                    starObj.transform.position = randPoses(otherLayerPos, 2);
                    otherLayerPos = starObj.transform.position;
                }
                for(int i=0; i<num; i++) {
                    GameObject starObj = Instantiate(starPrefs[starImgIndx]);
                    starObj.transform.position = randPoses(nextStarPos, 2);

                    nextStarPos = new Vector2(starObj.transform.position.x, 27);
                }
                break;
        }
        mountainIndex++;
    }

    //다음 지형(산, 구름, 별)
    public void GenerateNextSet() {
        GameObject obj = Instantiate(skyPref);
        obj.transform.position = nextSetPos;
        obj.transform.SetParent(holder, false);

        obj = Instantiate(groundPref);
        obj.transform.position = new Vector2(nextSetPos.x, 0);
        obj.transform.SetParent(holder, false);

        nextSetPos = new Vector2(nextSetPos.x + 66.5f, nextSetPos.y);

        Generate(10, 0);
        Generate(20, 1);
        Generate(20, 2);
    }
}
