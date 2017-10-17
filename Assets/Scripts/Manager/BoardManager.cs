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
    //좌측 기준 현재 폴의 위치
    private Vector2 curFlagPos;
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

            Debug.Log(lastTilePos);
        }
        initFlag();
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

    //초기 폴 추가
    private void initFlag() {
        for(int i=fstFlagAppearTile; i<=columns-1; i++) {
            GameObject leftFlag = Instantiate(flagPref);
            leftFlag.GetComponent<FlagController>().rayDir = FlagController.type.LEFT;
            float xPos = (float)rndX() / (float)gm.pixelPerUnit;

            leftFlag.transform.position = new Vector2(xPos, - i);

            GameObject rightFlag = Instantiate(flagPref);
            rightFlag.GetComponent<FlagController>().rayDir = FlagController.type.RIGHT;
            xPos = leftFlag.transform.position.x + ((float)gm.row_interval_default / (float)gm.pixelPerUnit);

            rightFlag.transform.position = new Vector3(xPos, - i);

            curFlagPos = leftFlag.transform.position;

            leftFlag.transform.SetParent(tiles[i].transform);
            rightFlag.transform.SetParent(tiles[i].transform);

            flagNum++;
        }
    }

    //동적 폴 추가
    public void addFlag() {
        for(int i=0; i<3; i++) {
            GameObject leftFlag = Instantiate(flagPref);
            leftFlag.GetComponent<FlagController>().rayDir = FlagController.type.LEFT;

            Vector2 nextPos = calcNextFlagPos();

            leftFlag.transform.position = nextPos;

            float rightX = (float)Math.Round(leftFlag.transform.position.x + ((float)gm.poll_interval_default / (float)gm.pixelPerUnit), 2);
            GameObject rightFlag = Instantiate(flagPref);
            rightFlag.GetComponent<FlagController>().rayDir = FlagController.type.RIGHT;

            rightFlag.transform.position = new Vector2(rightX, nextPos.y);

            curFlagPos = nextPos;

            flagNum++;
        }

        //폴 사이 간격 감소
        if (flagNum % gm.poll_interval_dec_per_num == 0) {
            lvup(0);
        }

        //행간 간격 증가
        if(flagNum % gm.row_interval_inc_per_num == 0) {
            lvup(1);
        }

        //행 평행이동
        if (flagNum % gm.row_total_move_per_num == 0) {
            lvup(2);
        }
    }

    private int rndX() {
        int rnd = UnityEngine.Random.Range((int)-gm.poll_interval_default, (int)(gm.pixelPerUnit - gm.poll_interval_default));
        return rnd;
    }

    private Vector2 calcNextFlagPos() {
        Vector2 prePos = curFlagPos;
        int[] arr = { 1, -1 };
        int val = arr.Random();

        float deltaX = UnityEngine.Random.Range(
            gm.row_total_default_min_move_amount + (gm.row_total_min_move_amount) * (row_parallel_move_lv - 1),
            gm.row_total_default_max_move_amount + (gm.row_total_max_move_amount) * (row_parallel_move_lv - 1)
        );

        float deltaY = gm.row_interval_default * (1 + gm.row_interval_inc_per_amount * row_interval_lv / 100);

        float unit = gm.pixelPerUnit;

        Vector2 nextPos = new Vector2(prePos.x + (float)Math.Round(deltaX/unit, 2) * val, prePos.y - (float)Math.Round(deltaY / unit, 2));
        return nextPos;
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