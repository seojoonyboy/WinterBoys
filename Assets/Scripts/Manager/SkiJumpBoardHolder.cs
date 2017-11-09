using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpBoardHolder : MonoBehaviour {
    public GameObject[] 
        mountainPrefs,
        cloudPrefs;
    
    //생성된 프리팹간 간격
    public float 
        intervalsOfMountains,
        intervalOfClouds;

    //한번 생성시 몇개씩 생성할 것인지.
    public int
        bundleOfMountains = 1,
        bundleOfClouds = 1;

    //한번에 2개 이상 생성할 때 서로간 상대적 무작위 간격(상하좌우)을 위한 상수
    public int[] 
        offSetOfMountain,
        offSetOfCloud;

    private void Awake() {
        
    }
}
