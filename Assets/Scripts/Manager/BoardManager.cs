using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    private GameManager gm;
    public GameObject[] floorsPref;
    public GameObject flagPref;

    public int columns = 3;
    public Transform floorHolder;

    public Vector2 
        lastTilePos,
        firstTilePos;

    public bool isMade = false;
    public bool isFlagMade = false;
    private int fstFlagAppearTile = 3;
    private int floorIndex = 2;
    
    private int nextFlagDir = 1;    //다음 폴 생성 방향 (-1 : 왼쪽 / 1 : 오른쪽)
    private int sameDirCount = 0;   //같은 방향으로 폴이 생성된 횟수

    private Vector2 curFlagPos;     //좌측 기준 현재 폴의 위치

    public int 
        flagNum = 0,
        poll_interval_lv = 1,       //폴과 폴 사이의 간격
        row_interval_lv = 1,        //행간 간격
        row_parallel_move_lv = 1;   //폴의 평행이동

    List<GameObject> tiles = new List<GameObject>();

    private void OnEnable() {
        gm = GameManager.Instance;
        setUp();
    }

    public void setUp() {
        for(int i=0; i<=columns - 1; i++) {
            GameObject floor;
            if (i == columns - 1) {
                floor = Instantiate(floorsPref[1]);

                floor.transform.SetParent(floorHolder, false);
                floor.transform.position = new Vector2(0, -i);

                lastTilePos = floor.transform.position;
            }
            else {
                floor = Instantiate(floorsPref[i]);

                floor.transform.SetParent(floorHolder, false);
                floor.transform.position = new Vector2(0, -i);

                if (i == 0) {
                    firstTilePos = floor.transform.position;
                }
            }
            tiles.Add(floor);
        }
        curFlagPos = new Vector2(0, -4);
        addFlag();
    }

    public void addToBoard() {
        isMade = true;

        //다음 타일 생성
        GameObject newFloor = Instantiate(floorsPref[floorIndex]);
        if(floorIndex >= columns - 2) {
            floorIndex = 1;
        }
        else {
            floorIndex++;
        }
        newFloor.transform.SetParent(floorHolder, false);
        newFloor.transform.position = new Vector2(0, lastTilePos.y - 1);

        //첫번째 타일 제거
        resetArr(newFloor);

        //다음 폴 생성
        addFlag();
    }

    private void resetArr(GameObject newFloor) {
        Destroy(tiles[0].gameObject);
        tiles.RemoveAt(0);

        tiles.Add(newFloor);

        lastTilePos = newFloor.transform.position;
        firstTilePos = tiles[0].transform.position;

        isMade = false;
    }

    //동적 폴 추가
    public void addFlag() {
        for(int i=0; i<3; i++) {
            GameObject leftFlag = Instantiate(flagPref);
            leftFlag.GetComponent<FlagController>().rayDir = FlagController.type.LEFT;
            //좌측 폴의 다음 위치 계산
            Vector2 nextPos = calcNextFlagPos();
            leftFlag.transform.position = nextPos;

            //우측 폴의 다음 위치 계산
            float deltaX = gm.poll_intervals[0] * (1 - ((gm.poll_intervals[2] * (poll_interval_lv - 1)) / 100));
            float rightX = (float)Math.Round(leftFlag.transform.position.x + (float)(deltaX / gm.pixelPerUnit), 2);
            GameObject rightFlag = Instantiate(flagPref);

            leftFlag.GetComponent<FlagController>().distance = deltaX;

            rightFlag.GetComponent<FlagController>().rayDir = FlagController.type.RIGHT;

            rightFlag.transform.position = new Vector2(rightX, nextPos.y);

            curFlagPos = nextPos;

            flagNum++;
        }

        //폴 사이 간격 감소
        if (flagNum != 0 && flagNum % gm.poll_intervals[1] == 0) {
            lvup(0);
        }

        //행간 간격 증가
        if(flagNum != 0 && flagNum % gm.vertical_intervals[1] == 0) {
            lvup(1);
        }

        //행 평행이동
        if (flagNum != 0 && flagNum % gm.pararell_intervals[2] == 0) {
            lvup(2);
        }
    }

    private int rndX() {
        int rnd = UnityEngine.Random.Range((int)-gm.poll_intervals[0], (int)(gm.pixelPerUnit - gm.poll_intervals[0]));
        return rnd;
    }

    private Vector2 calcNextFlagPos() {
        Vector2 prePos = curFlagPos;

        float deltaX = UnityEngine.Random.Range(
            gm.pararell_intervals[0] + (gm.pararell_intervals[2]) * (row_parallel_move_lv - 1),
            gm.pararell_intervals[1] + (gm.pararell_intervals[3]) * (row_parallel_move_lv - 1)
        );

        float deltaY = gm.vertical_intervals[0] * (1 + gm.vertical_intervals[1] * row_interval_lv / 100);
        float unit = gm.pixelPerUnit;

        int nextDir = setNextDir();
        float nextXPos = prePos.x + (float)Math.Round(deltaX / unit, 2) * nextDir;
        float rightX = (float)Math.Round(nextXPos + ((float)gm.poll_intervals[0] / (float)gm.pixelPerUnit), 2);

        if (nextXPos <= -0.9f || rightX >= 0.9f) {
            nextXPos = prePos.x + (float)Math.Round(deltaX / unit, 2) * nextDir * -1f;
            sameDirCount = 0;
            nextDir *= -1;
        }

        Vector2 nextPos = new Vector2(nextXPos, prePos.y - (float)Math.Round(deltaY / unit, 2));

        //Debug.Log(nextXPos);
        return nextPos;
    }

    private int setNextDir() {
        int[] arr = { 1, -1 };
        int nextDir = arr.Random();
        //Debug.Log("다음 방향 : " + nextDir);
        //Debug.Log("같은 방향 수 : " + sameDirCount);
        if (sameDirCount >= 2) {
            nextDir *= -1;
            sameDirCount = 0;
        }

        if (nextFlagDir == nextDir) {
            sameDirCount++;
        }
        return nextDir;
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
                Debug.Log("폴 간격 Lv 증가");
                poll_interval_lv++;
                break;
            case 1:
                Debug.Log("행 간격 Lv 증가");
                row_interval_lv++;
                break;
            case 2:
                Debug.Log("폴 평행이동 Lv 증가");
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