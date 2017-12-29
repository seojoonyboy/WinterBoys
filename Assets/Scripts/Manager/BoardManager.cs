using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private int preFlagType = 1;
    private int preSideTileIndex = 0;

    private GameManager gm;
    public GameObject[] 
        Tiles,
        flagPrefs;
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
        preSideTileIndex = getStartTileIndex();
        for (int i=0; i<columns; i++) {
            addToBoard();
            addFlag();
        }
    }

    private int getStartTileIndex() {
        int[] arr = { 0, 1, 3, 5 };
        int randNum = arr.Random();
        return randNum;
    }

    public void addToBoard() {
        GameObject floor = Instantiate(Tiles[preSideTileIndex]);
        floor.transform.SetParent(floorHolder, false);
        floor.transform.position = lastTilePos;

        lastTilePos = new Vector2(lastTilePos.x, lastTilePos.y - 7.1f);

        if (preSideTileIndex == 1) {
            preSideTileIndex = 2;
        }
        else if (preSideTileIndex == 3) {
            preSideTileIndex = 4;
        }
        else {
            preSideTileIndex = getStartTileIndex();
        }
    }

    //동적 폴 추가
    public void addFlag() {
        flagInterval = (gm.vertical_intervals[0] / gm.pixelPerUnit) * (1 + gm.vertical_intervals[1] * row_interval_lv / 100f);

        GameObject flag = Instantiate(flagPref);
        if(preFlagType == 1) {
            flag.GetComponent<FlagController>().flagType = FlagController.type.RIGHT;
        }
        else if(preFlagType == -1) {
            flag.GetComponent<FlagController>().flagType = FlagController.type.LEFT;
        }
        preFlagType *= -1;

        flag.transform.position = new Vector2(lastFlagPos.x, lastFlagPos.y - flagInterval);
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