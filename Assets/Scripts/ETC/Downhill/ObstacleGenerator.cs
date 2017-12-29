using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class ObstacleGenerator : MonoBehaviour {
    //unit 단위 기준으로 하였음
    public Ski_PlayerController controller;
    public GameObject tree;
    public float firstShown;
    public float interval;
    public int maxDensity;
    private Vector2 lastTreePos;
    private void Start() {
        lastTreePos = new Vector2(0, -firstShown);
    }

    private void Update() {
        float charPosOfY = controller.transform.position.y;
        if(charPosOfY < lastTreePos.y + 10) {
            generate();
        }
    }

    private void generate() {
        Debug.Log("나무 생성");
        int[] arr = { 1, maxDensity };
        float randNum = arr.Random();
        for(int i=0; i<=randNum; i++) {
            GameObject treeObj = Instantiate(tree);
            int[] arr2 = { 1, 2 };
            int randIndex = arr2.Random();
            if (randIndex == 1) {
                treeObj.GetComponent<SkeletonAnimation>().AnimationName = "blue";
                treeObj.GetComponent<TreeHandler>().type = TreeHandler.TreeType.BLUE;
            }
            else {
                treeObj.GetComponent<SkeletonAnimation>().AnimationName = "green";
                treeObj.GetComponent<TreeHandler>().type = TreeHandler.TreeType.GREEN;
            }

            float randX = UnityEngine.Random.Range(-1.3f, 1.3f);
            float randY = UnityEngine.Random.Range(-1.0f, 1.0f);

            treeObj.transform.position = new Vector3(randX, lastTreePos.y + randY);
        }

        lastTreePos = new Vector2(0, lastTreePos.y - interval);
    }
}
