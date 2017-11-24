using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using UnityEngine.UI;

public class Ski_PlayerController : MonoBehaviour {
    public float speedForce = 0.8f;             //캐릭터 가속도의 힘
    private float statBasedSpeedForce;          //Stat을 적용한 가속도의 힘
    public float driftFactor = 1.0f;
    public float angleV = 45.0f;
    public float input_sensitive = 5.0f;        //캐릭터 회전력
    private float statBasedRotSenstive;         //Stat을 적용한 캐릭터 회전력
    public int rotateDir = 1;
    bool buttonDown = false;

    public Downhill_itemType playerState;

    public GameObject playerImage;
    public GameObject[]
        blue_chars,
        red_chars,
        yellow_chars;
    public Sprite[] plates;

    public BoardManager bM;
    public DownhillManager dM;
    private GameManager gm;
    private PointManager pm;

    private int characterIndex = 0;
    private GameObject[] selectedCharacters;
    private GameObject preObj;

    public Vector3 playerPos;

    private Rigidbody2D rb;

    private void Awake() {
        gm = GameManager.Instance;
        pm = PointManager.Instance;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start() {
        characterIndex = gm.character;

        if(characterIndex == 0) {
            selectedCharacters = blue_chars;
        }
        else if(characterIndex == 1) {
            selectedCharacters = red_chars;
        }
        else {
            selectedCharacters = yellow_chars;
        }
        preObj = selectedCharacters[0];

        selectedCharacters[0].SetActive(true);
        transform.Find("Plate").GetComponent<SpriteRenderer>().sprite = plates[characterIndex];

        statBasedSpeedForce = speedForce * pm.getSpeedPercent();
        statBasedRotSenstive = input_sensitive * pm.getControlPercent();
    }

    private void Update() {
        playerPos = selectedCharacters[0].transform.position;
    }

    private void FixedUpdate() {
        rb.AddForce(ForwardForce());

        if (buttonDown) {
            rb.angularVelocity += statBasedRotSenstive * rotateDir;
        }
        else {
            rb.angularVelocity = 0;
        }

        float angle = transform.eulerAngles.z;
        //최대회전 각도 지정 (우측)
        if (angle > 260) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 259.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        //최대회전 각도 지정 (좌측)
        if (angle < 100) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 100.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        angle = Vector3.Angle(transform.up, -Vector3.up);

        //특정 각도 (angleV)에 따른 미끄러짐 효과 부여
        if (angle >= angleV) {
            Vector3 val;
            if (transform.up.x < 0) {
                val = new Vector3(-1, 0.6f, 0) * (angle / 150.0f);
            } else {
                val = new Vector3(1, 0.6f, 0) * (angle / 150.0f);
            }
            rb.AddForce(val);
        }
        else {
            rb.velocity = AdditionalForce() + RightVelocity() * driftFactor;
        }

        changePlayerImage();
    }

    //전방으로의 가속도 부여
    Vector3 ForwardForce() {
        return transform.up * Vector3.Dot(-Vector3.up, transform.up) * statBasedSpeedForce;
    }

    //추가적인 전방으로의 속도
    Vector2 AdditionalForce() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    //코너링시 코너링 방향으로 밀리는 힘의 크기 (현재 속도 기준)
    Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

    void changePlayerImage() {
        var eularAngle = transform.eulerAngles;

        if (eularAngle.z >= 135 && eularAngle.z <= 225) {
            offSpines(0);
        }

        else if (eularAngle.z < 135) {
            if (eularAngle.z > 105) {
                offSpines(1);
            }
            else {
                offSpines(2);
            }
        }
        else if(eularAngle.z > 225) {
            if (eularAngle.z > 250) {
                offSpines(4);
            }
            else {
                offSpines(3);
            }
        }
    }

    public void OnPointerDown(int i) {
        //다른 방향버튼이 눌리고 있는 중인 경우
        if (buttonDown) {
            return;
        }
        rotateDir = i;
        buttonDown = true;
    }

    public void OnPointerUp() {
        buttonDown = false;
    }

    private void offSpines(int index) {
        if (preObj != selectedCharacters[index]) {
            preObj.SetActive(false);
        }
        selectedCharacters[index].SetActive(true);
        preObj = selectedCharacters[index];
    }
}