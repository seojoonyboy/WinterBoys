using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateMachine : MonoBehaviour {
    //다운힐 : 부스팅발판, 곰, 나무, 날벌레떼, 박힌 폴, 기름
    public int arrayNum = 0;
    public BitArray array;

    private void Start() {
        array = new BitArray(arrayNum);
    }
}
