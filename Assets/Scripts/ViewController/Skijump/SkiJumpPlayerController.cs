using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class SkiJumpPlayerController : MonoBehaviour {
    private GameManager gm;
    private PointManager pm;

    public SkiJumpManager sm;
    public Transform arrow;
    public Transform startPos;

    public float forceAmount;

    private Rigidbody2D rb;
    private int type = 0;
    private float jumpAmount = 15;
    private float rotateAmount = 35;        //회전력
    private float statBasedRotAmount;       //Stat을 적용한 회전력
    //최대 상승 가능 높이
    private float MaxHeight = 100;

    private bool
        isAscending = false,
        isLanding = false,
        isDescending = false,
        tmp = false;

    private int ascendingCnt = 0;
    private int characterIndex = 0;
    public GameObject[] characters;
    private SkeletonAnimation anim;
    private void Awake() {
        gm = GameManager.Instance;
        pm = PointManager.Instance;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        statBasedRotAmount = rotateAmount * pm.getControlPercent(SportType.SKIJUMP);
    }

    private void OnEnable() {
        characterIndex = gm.character;

        initChar(characterIndex);                   //Spine Character 설정
        anim = characters[characterIndex].GetComponent<SkeletonAnimation>();
        SkelAnimChange("starting", false);

        ArrowRotate.OnRotatingEnd += RotatingEnd;
        Landing.OnLanding += _OnLanding;

        isDescending = false;
        isAscending = false;
        isLanding = false;
        tmp = false;

        ascendingCnt = 0;

        Time.fixedDeltaTime = 0.02f;                //슬로우모션 제거
    }

    private void OnDisable() {
        ArrowRotate.OnRotatingEnd -= RotatingEnd;
        Landing.OnLanding -= _OnLanding;
    }

    private void Update() {
        sm.speedText.GetComponent<Text>().text = System.Math.Round(rb.velocity.magnitude * 3, 2) + " km/h";
    }

    private void FixedUpdate() {
        if (isLanding) return;

        float angle = transform.eulerAngles.z;

        if(rb.velocity.magnitude >= 20) {
            if(anim.AnimationName == "run") {
                SkelAnimChange("run_loop", true);
            }
        }

        if (tmp) {
            //반시계방향 회전중
            if (rb.angularVelocity > 0) {
                //Debug.Log("반시계 방향 회전중");
                if(angle > 45 && angle <= 180) {
                    transform.eulerAngles = new Vector3(0, 0, angle - 0.1f);
                    rb.angularVelocity = 0;
                }
            }
            //시계방향 회전중
            else {
                //Debug.Log("시계 방향 회전중");
                if(angle < 305 && angle >= 180) {
                    transform.eulerAngles = new Vector3(0, 0, angle + 0.1f);
                    rb.angularVelocity = 0;
                }
            }
        }

        if (isAscending) {
            //45도 이상 뒤로 기울지 않게 고정
            if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                rb.angularVelocity = statBasedRotAmount;
            }

            if (rb.velocity.y <= 0) {
                rb.AddForce(Vector2.up * rb.velocity.magnitude);
            }
            else {
                if (transform.position.y <= MaxHeight) {
                    rb.AddForce(Vector2.up * 20f);
                }
                else {
                    isAscending = false;
                }
            }
        }

        else {
            //하강 버튼을 누르는 경우
            if (isDescending) {
                if ((angle <= 45 && angle >= 0) || (angle <= 360 && angle >= 305)) {
                    rb.angularVelocity = -statBasedRotAmount;
                }

                Vector2 val = new Vector2(rb.velocity.x * 0.1f, -0.01f);
                rb.AddForce(val);
            }
            //자연 하강을 하는 경우
        }
    }

    private void RotatingEnd() {
        rb.AddForce(arrow.transform.right * forceAmount, ForceMode2D.Force);
        tmp = true;
    }

    public void Ascending() {
        isAscending = true;

        if (ascendingCnt > 1) {
            MaxHeight = transform.position.y * 0.65f;
        }
        else {
            MaxHeight = transform.position.y * 0.8f;
        }
        ascendingCnt++;
    }

    public void EndAscending() {
        isAscending = false;
    }

    public void Descending() {
        isDescending = true;
    }

    public void EndDescending() {
        isDescending = false;
    }

    //착지
    private void _OnLanding() {
        isLanding = true;
    }

    private void initChar(int index) {
        for(int i=0; i<characters.Length; i++) {
            if(i == index) {
                characters[i].SetActive(true);
            }
            else {
                characters[i].SetActive(false);
            }
        }
    }

    public void SkelAnimChange(string name, bool needLoop = false) {
        anim.loop = needLoop;
        anim.AnimationName = name;
    }
}
