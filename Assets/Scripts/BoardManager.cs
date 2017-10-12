using System.Collections;
using System.Collections.Generic;
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
    private float firstFlagY = -3.0f;
    
    List<GameObject> tiles = new List<GameObject>();
    private void Awake() {
        gm = GameManager.Instance;
        setUp();
    }

    public void setUp() {
        for(int i=0; i<=columns - 1; i++) {
            GameObject floor = Instantiate(floorsPref[0]);
            floor.transform.SetParent(floorHolder, false);
            floor.transform.position = new Vector2(0, -i);
            
            if(i == 0) {
                firstTilePos = floor.transform.position;
            }
            if(i == columns - 1) {
                lastTilePos = floor.transform.position;
            }

            tiles.Add(floor);
        }

        initFlag();
    }

    public void addToBoard() {
        isMade = true;
        //다음 타일 생성
        GameObject newFloor = Instantiate(floorsPref[0]);
        newFloor.transform.SetParent(floorHolder, false);
        newFloor.transform.position = new Vector2(0, lastTilePos.y - 1);

        //첫번째 타일 제거
        resetArr(newFloor);
    }

    private void resetArr(GameObject newFloor) {
        Destroy(tiles[0].gameObject);
        tiles.RemoveAt(0);

        tiles.Add(newFloor);

        lastTilePos = newFloor.transform.position;
        firstTilePos = tiles[0].transform.position;

        isMade = false;
    }

    private void initFlag() {
        GameObject leftFlag = Instantiate(flagPref);
        float xPos = (float)rndX() / (float)gm.pixelPerUnit;

        leftFlag.transform.position = new Vector2(xPos, firstFlagY);

        GameObject rightFlag = Instantiate(flagPref);
        xPos = leftFlag.transform.position.x + ((float)gm.row_interval_default / (float)gm.pixelPerUnit);

        rightFlag.transform.position = new Vector3(xPos, firstFlagY);
    }

    private int rndX() {
        int rnd = Random.Range((int)-gm.poll_interval_default, (int)(gm.pixelPerUnit - gm.poll_interval_default));
        Debug.Log(rnd);
        return rnd;
    }
}
