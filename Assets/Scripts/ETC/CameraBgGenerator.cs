using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBgGenerator : MonoBehaviour {
    public GameObject[] stars;
    private Camera cam;
    public int objNum = 10;

    private void Start() {
        cam = Camera.main;
        
        Vector3 pos = Vector3.zero;

        for (int i=0; i<objNum; i++) {
            float randX = Random.Range(-23f, 23f);
            float randY = Random.Range(-13f, 13f);

            int randIndex = Random.Range(0, stars.Length);
            GameObject obj = Instantiate(stars[randIndex]);
            obj.transform.SetParent(cam.transform);
            obj.transform.position = Vector3.zero;
            Debug.Log(randX + " , " + randY);
            obj.transform.position = new Vector3(randX, randY, 10f);
        }
    }
}
