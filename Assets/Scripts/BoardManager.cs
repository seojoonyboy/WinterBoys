using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public GameObject[] floorsPref;
    public int columns = 3;
    public Transform floorHolder;

    public Vector2 
        lastTilePos,
        firstTilePos;

    public bool isMade = false;

    List<GameObject> tiles = new List<GameObject>();
    private void Awake() {
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
}
