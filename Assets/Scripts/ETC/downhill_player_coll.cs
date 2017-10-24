using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downhill_player_coll : MonoBehaviour {
    private DownhillManager dM;
    private void Awake() {
        dM = GameObject.Find("Manager").GetComponent<DownhillManager>();
    }
}
