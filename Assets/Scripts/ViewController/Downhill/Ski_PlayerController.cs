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
    public float virtualPlayerPosOfY;

    private float
        boostCoolTime,
        speedReduceCoolTime,
        speedZeroCoolTime,
        reverseCoolTime,
        rotateIncCoolTime,
        rotateDecCoolTime,
        bounceCoolTime;

    bool 
        buttonDown = false,
        isBoucing = false;

    public PlayerState playerState;

    public GameObject playerImage;
    public GameObject[]
        blue_chars,
        red_chars,
        yellow_chars;
    public Sprite[] plates;

    public BoardManager bM;
    public DownhillManager dM;
    public IndicatorController ic;

    private GameManager gm;
    private SaveManager pm;

    private int characterIndex = 0;
    private GameObject[] selectedCharacters;
    private GameObject preObj;

    public Vector3 
        playerPos,
        bouceDir;

    private Rigidbody2D rb;
    private float additionalForceByEffect = 1f;
    private float additionalAngularForceByEffect = 1.0f;

    public AudioSource audioSource;
    private Quaternion beginQuarternion;
    private Vector2 beginEular;
    [SerializeField] private Material[] materials;
    private void Awake() {
        gm = GameManager.Instance;
        pm = SaveManager.Instance;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start() {
        offSpines(-1);
        characterIndex = CharacterManager.Instance.currentCharacter;

        TrailRenderer tr_left = transform.Find("Trail_left").GetComponent<TrailRenderer>();
        TrailRenderer tr_right = transform.Find("Trail_right").GetComponent<TrailRenderer>();
        //characterIndex = 2;
        if (characterIndex == 0) {
            selectedCharacters = blue_chars;
            tr_left.material = materials[0];
            tr_right.material = materials[0];
        }
        else if(characterIndex == 1) {
            selectedCharacters = red_chars;
            tr_left.material = materials[1];
            tr_right.material = materials[1];
        }
        else {
            selectedCharacters = yellow_chars;
            tr_left.material = materials[2];
            tr_right.material = materials[2];
        }
        preObj = selectedCharacters[0];

        selectedCharacters[0].SetActive(true);
        transform.Find("Plate").GetComponent<SpriteRenderer>().sprite = plates[characterIndex];

        statBasedSpeedForce = speedForce * pm.getSpeedPercent();
        statBasedRotSenstive = input_sensitive * pm.getControlPercent();

        boostCoolTime = 3.0f;
        speedReduceCoolTime = 5.0f;
        speedZeroCoolTime = 1.5f;
        reverseCoolTime = 7.0f;
        rotateIncCoolTime = 7.0f;
        rotateDecCoolTime = 7.0f;
        bounceCoolTime = 0.2f;

        playerState = PlayerState.NORMAL;

        beginQuarternion = transform.rotation;
    }

    public void bounce(Vector3 dir) {
        if (isBoucing) {
            rb.velocity = Vector3.zero;
            return;
        }

        bouceDir = dir;
        isBoucing = true;
    }

    private void Update() {
        playerPos = selectedCharacters[0].transform.position;
    }

    private void FixedUpdate() {
        if (dM.getTimeScale == 0) {
            rb.velocity = Vector3.zero;
            return;
        }

        //sideTile에 부딪힌 경우 캐릭터의 정면벡터의 반대 방향으로 순간 힘을 가한다.
        if (isBoucing) {
            bounceCoolTime -= Time.deltaTime;
            rb.velocity *= 0.8f;
            if(rb.velocity.y < 0) {
                rb.velocity = new Vector2(rb.velocity.x, 0.01f);
            }
            rb.AddForce(bouceDir * 10);
            if (bounceCoolTime < 0) {
                isBoucing = false;
                bounceCoolTime = 0.2f;
            }
            return;
        }

        if (playerPos.y < bM.lastFlagPos.y + 10) {
            bM.addFlag();
        }
        virtualPlayerPosOfY = -1 * selectedCharacters[0].transform.position.y * 3.0f;

        rb.AddForce(ForwardForce() * additionalForceByEffect);

        if(rb.velocity.y >= 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        //부스팅 효과
        if(playerState == PlayerState.BOOSTING) {
            boostCoolTime -= Time.deltaTime;
            if(boostCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                boostCoolTime = 3.0f;
                additionalForceByEffect = 1.0f;
            }
            else {
                rb.AddForce(AdditionalForceByEffect(additionalForceByEffect));
            }
        }

        //눈덩이 과속방지턱 효과
        if(playerState == PlayerState.SPEED_REDUCING) {
            speedReduceCoolTime -= Time.deltaTime;
            if(speedReduceCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                speedReduceCoolTime = 5.0f;
                additionalForceByEffect = 1.0f;
            }
            else {
                rb.AddForce(AdditionalForceByEffect(additionalForceByEffect));
            }
        }

        //곰 충돌 효과
        if(playerState == PlayerState.SPEED_ZERO) {
            speedZeroCoolTime -= Time.deltaTime;
            if(speedZeroCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                speedZeroCoolTime = 1.5f;
                additionalForceByEffect = 1.0f;
            }
            else {
                rb.AddForce(AdditionalForceByEffect(additionalForceByEffect));
            }
        }

        //날벌레 효과
        if(playerState == PlayerState.REVERSE_ROTATE) {
            reverseCoolTime -= Time.deltaTime;
            if(reverseCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                reverseCoolTime = 7.0f;
            }
        }

        //눈에 박혀있는 폴 효과
        if (playerState == PlayerState.ROTATING_INC) {
            rotateIncCoolTime -= Time.deltaTime;
            if(rotateIncCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                rotateIncCoolTime = 7.0f;
                additionalAngularForceByEffect = 1.0f;
            }
        }

        //눈에 뿌려진 검은 기름 효과
        if (playerState == PlayerState.ROTATING_DEC) {
            rotateDecCoolTime -= Time.deltaTime;
            if(rotateDecCoolTime < 0) {
                playerState = PlayerState.NORMAL;
                rotateDecCoolTime = 7.0f;
                additionalAngularForceByEffect = 1.0f;
            }
        }

        if (buttonDown) {
            audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_dirChange").clip;

            if (!audioSource.isPlaying) {
                audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_dirChange").clip;
                audioSource.Play();
            }

            if (playerState == PlayerState.REVERSE_ROTATE) {
                rb.angularVelocity += statBasedRotSenstive * -rotateDir * additionalAngularForceByEffect;
            }
            else {
                rb.angularVelocity += statBasedRotSenstive * rotateDir * additionalAngularForceByEffect;
            }
        }
        else {
            rb.angularVelocity = 0;

            audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_move").clip;

            if (!audioSource.isPlaying) {
                audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_move").clip;
                audioSource.Play();
            }
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

    Vector2 AdditionalForceByEffect(float amount) {
        return transform.up * amount;
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

    private void offSpines(int index = -1) {
        if(index == -1) {
            foreach(GameObject obj in blue_chars) {
                obj.SetActive(false);
            }
            foreach(GameObject obj in yellow_chars) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in red_chars) {
                obj.SetActive(false);
            }
        }
        else {
            if (preObj != selectedCharacters[index]) {
                preObj.SetActive(false);
            }
            selectedCharacters[index].SetActive(true);
            preObj = selectedCharacters[index];
        }
    }

    public void resetQuarternion() {
        transform.rotation = beginQuarternion;
        rb.angularVelocity = 0;
    }

    public void itemCheck(GameObject obj) {
        if (obj.tag == "Item") {
            Item item = obj.GetComponent<Item>();
            switch (item.item_dh) {
                case ItemType.DH.BOOSTING_HILL:
                    playerState = PlayerState.BOOSTING;
                    additionalForceByEffect = 1.0f;
                    break;
                case ItemType.DH.POINT:
                    dM.scoreInc(50);
                    break;
                case ItemType.DH.ANTI_SPEED_HILL:
                    playerState = PlayerState.SPEED_REDUCING;
                    additionalForceByEffect = 0.3f;
                    break;
                case ItemType.DH.ENEMY_BEAR:
                    playerState = PlayerState.SPEED_ZERO;
                    additionalForceByEffect = 0;
                    break;
                case ItemType.DH.ENEMY_BUGS:
                    playerState = PlayerState.REVERSE_ROTATE;
                    break;
                case ItemType.DH.OBSTACLE_POLL:
                    playerState = PlayerState.ROTATING_INC;
                    additionalAngularForceByEffect = 1.5f;
                    break;
                case ItemType.DH.OBSTACLE_OIL:
                    playerState = PlayerState.ROTATING_DEC;
                    additionalAngularForceByEffect = 0.5f;
                    break;
                case ItemType.DH.MONEY:
                    dM.addCrystal(5);
                    break;
                case ItemType.DH.TIME:
                    dM.remainTime += 15;
                    break;
            }
            Destroy(obj);
        }
    }

    public enum PlayerState {
        NORMAL,
        BOOSTING,
        SPEED_REDUCING,
        SPEED_ZERO,
        REVERSE_ROTATE,
        ROTATING_INC,
        ROTATING_DEC
    }
}