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
        firstTilePos;

    public bool isMade = false;
    public bool isFlagMade = false;
    private int fstFlagAppearTile = 3;
    private int floorIndex = 0;
    
    private int nextFlagDir = 1;    //다음 폴 생성 방향 (-1 : 왼쪽 / 1 : 오른쪽)
    private int sameDirCount = 0;   //같은 방향으로 폴이 생성된 횟수

    private Vector2 curFlagPos;     //좌측 기준 현재 폴의 위치

    public int 
        flagNum = 0,
        poll_interval_lv = 1,       //폴과 폴 사이의 간격
        row_interval_lv = 1,        //행간 간격
        row_parallel_move_lv = 1;   //폴의 평행이동

    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> leftSides = new List<GameObject>();
    List<GameObject> rightSides = new List<GameObject>();

    private TreeOffset 
        treeLeftOffset,
        treeRightOffset;

    private float nextTreePosY = -2;

    private class TreeOffset {
        public float leftLimit;
        public float rightLimit;
    }

    private void OnEnable() {
        gm = GameManager.Instance;
        setUp();
    }

    public void setUp() {
        for(int i=0; i<=columns - 1; i++) {
            GameObject floor = Instantiate(floorsPref[0]);
            GameObject leftSide = Instantiate(leftPref[i]);
            GameObject rightSide = Instantiate(rightPref[i]);

            floor.transform.SetParent(floorHolder, false);
            floor.transform.position = new Vector2(0, -i);
            leftSide.transform.SetParent(floorHolder, false);
            leftSide.transform.position = new Vector2(-2.5f, -i);
            rightSide.transform.SetParent(floorHolder, false);
            rightSide.transform.position = new Vector2(2.5f, -i);

            if (i == columns - 1) {
                lastTilePos = floor.transform.position;
            }
            if (i == 0) {
                firstTilePos = floor.transform.position;

                treeLeftOffset = new TreeOffset();
                float val = leftSide.transform.position.x;
                treeLeftOffset.leftLimit = val;
                treeLeftOffset.rightLimit = (float)(val + leftSide.transform.localScale.x / 2.0f);

                treeRightOffset = new TreeOffset();
                val = rightSide.transform.position.x;

                treeRightOffset.leftLimit = (float)(val - rightSide.transform.localScale.x / 2.0f);
                treeRightOffset.rightLimit = treeRightOffset.leftLimit + rightSide.transform.localScale.x / 2.0f;
            }
            tiles.Add(floor);
            leftSides.Add(leftSide);
            rightSides.Add(rightSide);

            addTree(true);
        }
        curFlagPos = new Vector2(0, -4);
        addFlag();
    }

    public void addToBoard() {
        isMade = true;

        //다음 타일 생성
        GameObject newFloor = Instantiate(floorsPref[0]);
        GameObject leftSide = Instantiate(leftPref[floorIndex]);
        GameObject rightSide = Instantiate(rightPref[floorIndex]);

        if (floorIndex >= columns - 1) {
            floorIndex = 0;
        }
        else {
            floorIndex++;
        }
        newFloor.transform.SetParent(floorHolder, false);
        newFloor.transform.position = new Vector2(0, lastTilePos.y - 1);

        leftSide.transform.SetParent(floorHolder, false);
        leftSide.transform.position = new Vector2(-2.5f, lastTilePos.y - 1);
        rightSide.transform.SetParent(floorHolder, false);
        rightSide.transform.position = new Vector2(2.5f, lastTilePos.y - 1);

        //첫번째 타일 제거
        resetArr(newFloor, leftSide, rightSide);

        //다음 폴 생성
        addFlag();

        addTree();
    }

    private void resetArr(GameObject newFloor, GameObject leftSide, GameObject rightSide) {
        Destroy(tiles[0].gameObject);
        Destroy(leftSides[0].gameObject);
        Destroy(rightSides[0].gameObject);

        tiles.RemoveAt(0);
        rightSides.RemoveAt(0);
        leftSides.RemoveAt(0);

        tiles.Add(newFloor);
        leftSides.Add(leftSide);
        rightSides.Add(rightSide);

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

    private void addTree(bool isFirst = false) {
        float val = 0;
        if (isFirst) {
            for(int i = 0; i< columns - 1; i++) {
                val = -(i + 1);
                nextTreePosY = UnityEngine.Random.Range(val, val - 1);
                int nextImageIndex = UnityEngine.Random.Range(0, treeImages.Length);

                GameObject tree = Instantiate(treePref);
                tree.GetComponent<SpriteRenderer>().sprite = treeImages[nextImageIndex];

                float nextPosX = UnityEngine.Random.Range(treeLeftOffset.leftLimit, treeLeftOffset.rightLimit);
                tree.transform.position = new Vector2(nextPosX, nextTreePosY);

                nextImageIndex = UnityEngine.Random.Range(0, treeImages.Length);

                GameObject rightTree = Instantiate(treePref);
                nextPosX = UnityEngine.Random.Range(treeRightOffset.leftLimit, treeRightOffset.rightLimit);
                rightTree.transform.position = new Vector2(nextPosX, nextTreePosY);
                rightTree.GetComponent<SpriteRenderer>().sprite = treeImages[nextImageIndex];
            }
        }
        else {
            val = lastTilePos.y - 1;
            for (int i = 0; i < 3; i++) {
                nextTreePosY = UnityEngine.Random.Range(val, val - 1);
                int nextImageIndex = UnityEngine.Random.Range(0, treeImages.Length);

                GameObject tree = Instantiate(treePref);
                tree.GetComponent<SpriteRenderer>().sprite = treeImages[nextImageIndex];

                float nextPosX = UnityEngine.Random.Range(treeLeftOffset.leftLimit, treeLeftOffset.rightLimit);
                tree.transform.position = new Vector2(nextPosX, nextTreePosY);

                nextImageIndex = UnityEngine.Random.Range(0, treeImages.Length);

                GameObject rightTree = Instantiate(treePref);
                nextPosX = UnityEngine.Random.Range(treeRightOffset.leftLimit, treeRightOffset.rightLimit);
                rightTree.transform.position = new Vector2(nextPosX, nextTreePosY);
                rightTree.GetComponent<SpriteRenderer>().sprite = treeImages[nextImageIndex];
            }
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