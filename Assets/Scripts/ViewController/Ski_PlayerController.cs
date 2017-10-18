using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;

public class Ski_PlayerController : MonoBehaviour {
    public float speedForce = 1.0f;
    public float torqueForce = 1.0f;
    public float driftFactor = 1.0f;
    public float angleV = 45.0f;
    public float input_sensitive = 0.1f;
    public int rotateDir = 1;
    bool buttonDown = false;

    public GameObject playerImage;
    public GameObject[]
        blue_chars,
        red_chars,
        yellow_chars;

    public BoardManager bM;
    public DownhillManager dM;
    private GameManager gm;
    private int characterIndex = 0;
    private GameObject[] selectedCharacters;
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
        selectedCharacters[0].SetActive(true);
    }

    private void FixedUpdate() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        //rb.velocity = ForwardVelocity() + RightVelocity() * driftFactorSlippy;
        rb.AddForce(__ForwardVelocity());

        //rb.angularVelocity = Input.GetAxis("Horizontal") * torqueForce;
        if(rb.velocity.y >= 0) {
            rb.velocity = new Vector2(rb.velocity.x, -0.1f);
        }
        if (buttonDown) {
            rb.angularVelocity += input_sensitive * rotateDir;
        }
        checkPlayerPos();
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

    void checkPlayerPos() {
        Ray2D ray = new Ray2D(transform.position, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null) {
            if (hit.collider.tag == "Tile") {
                Vector2 pos = hit.collider.transform.position;
                if (pos.y <= bM.firstTilePos.y - 2) {
                    if (!bM.isMade) {
                        bM.addToBoard();
                    }
                }
            }
        }
        //타일 밖으로 벗어난 경우
        else {
            dM.modal.SetActive(true);
            gm.gameOver();
        }
        Debug.DrawRay(transform.position, Vector3.forward, Color.red);
    }

    void changePlayerImage() {
        var eularAngle = transform.eulerAngles;
        //Debug.Log(eularAngle);
        if (eularAngle.z >= 135 && eularAngle.z <= 225) {
            //sR.sprite = gm.players[0];
            offSpines();
            selectedCharacters[0].SetActive(true);
        }
        if (eularAngle.z < 135) {
            offSpines();
            if (eularAngle.z > 105) {
                selectedCharacters[1].SetActive(true);
            }
            else {
                selectedCharacters[2].SetActive(true);
            }
        }
        else if(eularAngle.z > 225) {
            offSpines();
            if (eularAngle.z > 250) {
                selectedCharacters[4].SetActive(true);
            }
            else {
                selectedCharacters[3].SetActive(true);
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

    private void offSpines() {
        foreach(GameObject obj in selectedCharacters) {
            obj.SetActive(false);
        }
    }
}
