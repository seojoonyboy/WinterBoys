using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using UnityEngine.UI;

public class Ski_PlayerController : MonoBehaviour {
    public float speedForce = 1.0f;
    public float torqueForce = 1.0f;
    public float driftFactor = 1.0f;
    public float angleV = 45.0f;
    public float input_sensitive = 0.3f;
    public int rotateDir = 1;
    bool buttonDown = false;

    public GameObject playerImage;
    public GameObject[]
        blue_chars,
        red_chars,
        yellow_chars;
    public Sprite[] plates;

    public BoardManager bM;
    public DownhillManager dM;
    private GameManager gm;

    private int characterIndex = 0;
    private GameObject[] selectedCharacters;
    private GameObject preObj;
    //private Vector3 tmp;
    public Vector3 playerPos;
    private void Awake() {
        gm = GameManager.Instance;
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
    }

    private void Update() {
        playerPos = transform.position;
    }

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(__ForwardVelocity());

        if(rb.velocity.y >= 0) {
            rb.velocity = new Vector2(rb.velocity.x, -0.1f);
        }
        if (buttonDown) {
            rb.angularVelocity += input_sensitive * rotateDir;
        }
        else {
            rb.angularVelocity = 0;
        }
        changePlayerImage();

        if (transform.eulerAngles.z > 260) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 259.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        //최대회전 각도 지정 (좌측)
        if (transform.eulerAngles.z < 100) {
            if (rb.angularVelocity == 0) {
                rb.constraints = RigidbodyConstraints2D.None;
                transform.eulerAngles = new Vector3(0, 0, 100.5f);
            } else {
                rb.freezeRotation = true;
            }
        }

        float angle = Vector3.Angle(transform.up, -Vector3.up);

        if (angle >= angleV) {
            //Debug.Log(transform.up.x);
            Vector3 val;
            if (transform.up.x < 0) {
                val = new Vector3(-1, 0.6f, 0) * (angle / 150.0f);
            } else {
                val = new Vector3(1, 0.6f, 0) * (angle / 150.0f);
            }
            rb.AddForce(val);
        }
        //rb.AddForce(ForwardVelocity() * 2f);
        else {
            rb.velocity = ForwardVelocity() + RightVelocity() * driftFactor;
        }
    }

    //전방으로 얼마나 추가적으로 힘을 가할지 (현재 속도 기준)
    Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
    }

    Vector3 __ForwardVelocity() {
        return transform.up * Vector3.Dot(-Vector3.up, transform.up) * speedForce;
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
