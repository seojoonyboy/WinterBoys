using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private GameManager gm;
    public GameObject[] 
        floorsPref,
        leftPref,
        rightPref;
    public Sprite[] treeImages;
    public GameObject 
        flagPref,
        treePref;

    public int columns = 3;
    public Transform floorHolder;

    public Vector2 
        lastTilePos,
        lastFlagPos;

    [SerializeField] private float flagInterval;

    public bool isMade = false;
    private int floorIndex = 0;
    
    private int nextFlagDir = 1;    //다음 폴 생성 방향 (-1 : 왼쪽 / 1 : 오른쪽)
    private int sameDirCount = 0;   //같은 방향으로 폴이 생성된 횟수
    public int 
        flagNum = 0,
        poll_interval_lv = 1,       //폴과 폴 사이의 간격
        row_interval_lv = 1,        //행간 간격
        row_parallel_move_lv = 1;   //폴의 평행이동

    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> leftSides = new List<GameObject>();
    List<GameObject> rightSides = new List<GameObject>();
    public List<GameObject> centers = new List<GameObject>();

    private void OnEnable() {
        gm = GameManager.Instance;
        setUp();
    }

    public void setUp() {
        flagInterval = gm.vertical_intervals[0] / gm.pixelPerUnit;
        for (int i=0; i<columns; i++) {
            addMiddleTile(-i * 7.1f);
            addSideTile();
            addFlag();
        }
    }

    private void addMiddleTile(float posOfY) {
        GameObject floor = Instantiate(floorsPref[0]);

        floor.transform.position = new Vector2(0, posOfY);
        floor.transform.SetParent(floorHolder, false);

        if (floorIndex >= columns - 1) {
            floorIndex = 0;
        }
        else {
            floorIndex++;
        }
        lastTilePos = floor.transform.position;
    }

    private void addSideTile() {
        GameObject leftSide = Instantiate(leftPref[floorIndex]);
        GameObject rightSide = Instantiate(rightPref[floorIndex]);

        leftSide.transform.SetParent(floorHolder, false);
        leftSide.transform.position = new Vector2(-2.0f, lastTilePos.y - 1);
        rightSide.transform.SetParent(floorHolder, false);
        rightSide.transform.position = new Vector2(2.0f, lastTilePos.y - 1);
    }

    public void addToBoard() {
        addMiddleTile(lastTilePos.y - 1 * 7.1f);
        addSideTile();
    }

    //동적 폴 추가
    public void addFlag() {
        GameObject flag = Instantiate(flagPref);
        flag.transform.position = new Vector2(lastFlagPos.x, lastFlagPos.y - (float)(gm.vertical_intervals[0]/gm.pixelPerUnit));
        lastFlagPos = flag.transform.position;
        flag.name = "flag" + flagNum;

        flag.transform.SetParent(floorHolder, false);
        flagNum++;

        //폴 사이 간격 감소
        if (flagNum != 0 && flagNum % gm.poll_intervals[1] == 0) {
            lvup(0);
        }

        //평행이동 간격 증가
        if(flagNum != 0 && flagNum % gm.vertical_intervals[1] == 0) {
            lvup(1);
        }
    }

    private void setFlagParent(GameObject leftFlag, GameObject rightFlag) {
        for(int i=0; i<tiles.Count; i++) {
            float top = tiles[i].transform.position.y + 0.5f;
            float bot = tiles[i].transform.position.y - 0.5f;

            float flagPosY = leftFlag.transform.position.y;
            if (flagPosY <= top && flagPosY >= bot) {
                leftFlag.transform.SetParent(tiles[i].transform);
                rightFlag.transform.SetParent(tiles[i].transform);
            }
        }
    }

    private void lvup(int index) {
        switch (index) {
            case 0:
                //Debug.Log("폴 간격 Lv 증가");
                poll_interval_lv++;
                break;
            case 1:
                //Debug.Log("행 간격 Lv 증가");
                row_interval_lv++;
                break;
            case 2:
                //Debug.Log("폴 평행이동 Lv 증가");
                row_parallel_move_lv++;
                break;
        }
    }
}

public static class RandomSelector {
    static System.Random random = new System.Random();
    public static T Random<T>(this IEnumerable<T> enumerable) {
        int c = enumerable.Count();
        int i = random.Next(c);
        return enumerable.Skip(i).First();
    }
}