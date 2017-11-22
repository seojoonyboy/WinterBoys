using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour {
    public type _type;
    public enum type { NULL, SKIJUMP_MAINCAM, SKIJUMP_CAM_TARGET }
    public Transform target;

    private void Update() {
        if(_type == type.SKIJUMP_MAINCAM) {
            //카메라의 하단이 바닥 영역을 넘어가거나

            //카메라의 상단이 하늘 영역을 벗어나지 않도록 고정

            //그밖의 경우
            GetComponent<Camera>().orthographicSize = 12f;
            transform.position = target.position;
        }
        else if(_type == type.SKIJUMP_CAM_TARGET) {
            transform.position = new Vector3(target.position.x + 3f, target.position.y + 1f, -10f);
        }
    }
}
