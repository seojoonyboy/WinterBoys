using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour {
    public type _type;
    public enum type { NULL, SKIJUMP_MAINCAM, SKIJUMP_CAM_TARGET }
    public Transform target;

    private float camSize;
    private void Awake() {
    }
    private void Update() {
        
    }

    private void FixedUpdate() {
        if (_type == type.SKIJUMP_MAINCAM) {
            Vector2 camPos = transform.position;
            Vector2 targetPos = target.position;
            camSize = GetComponent<Camera>().orthographicSize;
            //카메라의 상단이 하늘 영역을 벗어나지 않도록 고정
            GetComponent<Camera>().orthographicSize = 10f;
            if (targetPos.y > 23) {
                transform.position = new Vector3(target.position.x, transform.position.y, -10f);
            }

            //카메라의 하단이 바닥 영역을 벗어나지 않도록 고정
            else if (targetPos.y < 10) {
                transform.position = new Vector3(target.position.x, camSize, -10f);
            }

            //그밖의 경우
            else {
                transform.position = new Vector3(targetPos.x, targetPos.y, -10);
            }
        }
        else if (_type == type.SKIJUMP_CAM_TARGET) {
            transform.position = new Vector3(target.position.x + 3f, target.position.y + 1f, -10f);
        }
    }
}
