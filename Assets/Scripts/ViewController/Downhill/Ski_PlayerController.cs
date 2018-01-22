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
    public int rotateDir = 1;                   //좌측회전 버튼 : -1, 우측회전 버튼 : 1

    public float virtualPlayerPosOfY;

    private float
        boostCoolTime,
        speedReduceCoolTime,
        speedZeroCoolTime,
        treeSpeedZeroCoolTime,
        reverseCoolTime,
        rotateIncCoolTime,
        rotateDecCoolTime,
        bounceCoolTime;

    bool
        buttonDown = false,
        isBoucing = false,
        isPlayedDeadAnim = false;
    public GameObject 
        playerImage,
        plate,
        snowFricEffect,
        playerHeadBugEffect;

    public GameObject[]
        blue_chars,
        red_chars,
        yellow_chars,
        icons;

    public Sprite[] plates;

    public BoardManager bM;
    public DownhillManager dM;
    public ItemCoolTime itemCoolTimes;

    private GameManager gm;
    private SaveManager pm;

    private int characterIndex = 0;
    private GameObject[] selectedCharacters;
    private GameObject preObj;

    public Vector3 
        playerPos,
        bouceDir;

    public Rigidbody2D rb;
    private float additionalForceByEffect = 1f;
    private float 
        pollBuff = 1.4f,
        oilBuff = 0.6f;

    public AudioSource audioSource;
    private Quaternion beginQuarternion;
    private Vector2 beginEular;
    private SkeletonAnimation anim;
    private CharStateMachine stateMachine;
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

        beginQuarternion = transform.rotation;

        stateMachine = GetComponent<CharStateMachine>();
    }

    public void bounce(Vector3 amount) {
        rb.velocity = Vector3.zero;

        rb.AddForce(amount, ForceMode2D.Impulse);
        isBoucing = true;

        gm.vibrate();
    }

    private void Update() {
        playerPos = selectedCharacters[0].transform.position;
    }

    private void FixedUpdate() {
        if (dM.getTimeScale == 0) {
            rb.velocity = Vector3.zero;
            audioSource.loop = false;
            return;
        }

        changePlayerImage();

        if (stateMachine.array[3]) {
            if (!playerHeadBugEffect.activeSelf) {
                playerHeadBugEffect.SetActive(true);
            }
        }
        else {
            playerHeadBugEffect.SetActive(false);
        }
        Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        if(pos.x < 0 || pos.x > 1) {
            dM.OnGameOver(DownhillManager.GameoverReason.SIDETILE);
        }

        if (dM.isTimeUp) {
            rb.MoveRotation(180);
            rb.velocity *= 0.9995f;
            offSpines(5);
            //selectedCharacters[5].gameObject.SetActive(true);
            if (!audioSource.isPlaying) {
                audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_dirChange").clip;
                audioSource.Play();
            }

            if (!isPlayedDeadAnim) {
                //카메라 중앙 기준 왼쪽인 경우
                if(pos.x < 0.5f) {
                    //flip anim
                    anim.initialFlipX = true;
                }

                else {
                    anim.initialFlipX = false;
                }
                anim.Initialize(true);

                Spine.TrackEntry track = anim.AnimationState.SetAnimation(0, "die", false);
                Invoke("DeadAnimEnd", track.animationEnd);
                isPlayedDeadAnim = true;
            }
            return;
        }
        else {
            isPlayedDeadAnim = false;
        }

        //sideTile에 부딪힌 경우 캐릭터의 정면벡터의 반대 방향으로 순간 힘을 가한다.
        if (isBoucing) {
            bounceCoolTime -= Time.deltaTime;
            rb.velocity = rb.velocity * 0.8f;
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

        //부스팅 효과를 받는 중이면 추가 AddForce
        rb.AddForce(ForwardForce() * additionalForceByEffect);

        //항상 제자리나 아래방향으로 이동하도록 추가 제어
        if(rb.velocity.y >= 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        //부스팅 효과
        if(stateMachine.array[0]) {
            boostCoolTime -= Time.deltaTime;
            if(boostCoolTime < 0) {
                additionalForceByEffect = 1.0f;
                stateMachine.array.Set(0, false);
            }
        }

        //곰 충돌 효과
        if(stateMachine.array[1]) {
            speedZeroCoolTime -= Time.deltaTime;
            if(speedZeroCoolTime < 0) {
                stateMachine.array.Set(1, false);

            }
            rb.velocity = Vector2.zero;
        }

        //나무 충돌 효과
        if(stateMachine.array[2]) {
            treeSpeedZeroCoolTime -= Time.deltaTime;
            if(treeSpeedZeroCoolTime < 0) {
                stateMachine.array.Set(2, false);
            }
            else {
                rb.velocity = Vector2.zero;
            }
        }

        //날벌레 효과
        if(stateMachine.array[3]) {
            reverseCoolTime -= Time.deltaTime;
            if(reverseCoolTime < 0) {
                stateMachine.array.Set(3, false);

            }
        }

        //눈에 박혀있는 폴 효과
        if (stateMachine.array[4]) {
            rotateIncCoolTime -= Time.deltaTime;
            if(rotateIncCoolTime < 0) {
                stateMachine.array.Set(4, false);
                pollBuff = 1.0f;
            }
        }

        //눈에 뿌려진 검은 기름 효과
        if (stateMachine.array[5]) {
            rotateDecCoolTime -= Time.deltaTime;
            if(rotateDecCoolTime < 0) {
                stateMachine.array.Set(5, false);
                oilBuff = 1.0f;
            }
        }

        if (buttonDown) {
            if(dM.getTimeScale != 0) {
                audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_dirChange").clip;

                if (!audioSource.isPlaying) {
                    audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_dirChange").clip;
                    audioSource.Play();
                }
            }

            if (stateMachine.array[3]) {
                rb.angularVelocity += statBasedRotSenstive * -rotateDir * AdditionalAngularForce();
            }
            else {
                rb.angularVelocity += statBasedRotSenstive * rotateDir * AdditionalAngularForce();
            }
        }
        else {
            if (dM.getTimeScale != 0) {
                audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_move").clip;

                if (!audioSource.isPlaying) {
                    audioSource.clip = dM.soundManager.searchResource(SoundManager.SoundType.EFX, "dh_move").clip;
                    audioSource.Play();
                }
            }

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
    }

    float AdditionalAngularForce() {
        float result = 1;
        bool oilEffect = stateMachine.array[5];
        bool pollEffect = stateMachine.array[4];

        if (oilEffect) {
            if (pollEffect) {
                result = 1;
            }
            else {
                result = 0.6f;
            }
        }
        else {
            if (pollEffect) {
                result = 1.4f;
            }
            else {
                result = 1;
            }
        }

        return result;
    }

    //전방으로의 가속도 부여
    Vector3 ForwardForce() {
        Vector3 dotVal = transform.up * Vector3.Dot(-Vector3.up, transform.up) * statBasedSpeedForce;
        Vector3 dotVal2 = transform.right * Vector3.Dot(-Vector3.up, -transform.right) * statBasedSpeedForce;
        Vector3 forceVal = new Vector3(dotVal2.x, dotVal.y);
        return forceVal;
    }

    //추가적인 전방으로의 속도
    Vector2 AdditionalForce() {
        Vector2 forceVal = GetComponent<Rigidbody2D>().velocity;
        if (forceVal.x < 1) {
            forceVal = new Vector2(2.1f, forceVal.y);
        }
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

        anim = selectedCharacters[5].GetComponent<SkeletonAnimation>();
    }

    void DeadAnimEnd() {
        dM.OnGameOver(DownhillManager.GameoverReason.TIMEEND);
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

            //죽는 모션
            if(index == 5) {
                plate.SetActive(false);
                snowFricEffect.SetActive(false);
            }
            else {
                plate.SetActive(true);
                snowFricEffect.SetActive(true);
            }
        }
    }

    public void resetQuarternion() {
        transform.rotation = beginQuarternion;
        rb.angularVelocity = 0;
    }

    public void itemCheck(GameObject obj) {
        //부스팅 효과 적용중일 때 다른 효과 무시
        if(stateMachine.array[0]) {
            return;
        }
        //아이템 태그가 없는 경우 무시
        if(obj.tag != "Item") { return; }
        Item item = obj.GetComponent<Item>();
        //아이템 컴포넌트가 없는 경우 무시
        if (item == null) { return; }

        float cooltime = 0;
        switch (item.item_dh) {
            case ItemType.DH.BOOSTING_HILL:
                stateMachine.array.Set(0, true);

                boostCoolTime = itemCoolTimes.boosting_cooltime;
                cooltime = boostCoolTime;

                additionalForceByEffect = 1.5f;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_good");
                break;
            case ItemType.DH.ENEMY_BEAR:
                stateMachine.array.Set(1, true);

                speedZeroCoolTime = itemCoolTimes.speedZero_cooltime;
                cooltime = speedZeroCoolTime;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "dh_bigCrash");

                gm.vibrate();
                break;
            case ItemType.DH.TREE:
                stateMachine.array.Set(2, true);

                treeSpeedZeroCoolTime = itemCoolTimes.treeSpeedZero_cooltime;
                cooltime = treeSpeedZeroCoolTime;

                Vector3 forceDir = new Vector3(0, 2, 0);
                bounce(forceDir);

                dM.soundManager.Play(SoundManager.SoundType.EFX, "dh_tree");

                item.transform.Find("Image").GetComponent<TreeHandler>().Play();
                break;
            case ItemType.DH.ENEMY_BUGS:
                stateMachine.array.Set(3, true);

                reverseCoolTime = itemCoolTimes.reverseRot_cooltime;
                cooltime = reverseCoolTime;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_bad");
                break;
            case ItemType.DH.OBSTACLE_POLL:
                stateMachine.array.Set(4, true);

                rotateIncCoolTime = itemCoolTimes.increaseRot_cooltime;
                cooltime = rotateIncCoolTime;

                pollBuff = 1.4f;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_good");
                break;
            case ItemType.DH.OBSTACLE_OIL:
                stateMachine.array.Set(5, true);

                rotateDecCoolTime = itemCoolTimes.decreaseRot_cooltime;
                cooltime = rotateDecCoolTime;

                oilBuff = 0.6f;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_bad");
                break;

            case ItemType.DH.POINT:
                dM.scoreInc(50);

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_good");
                break;
            case ItemType.DH.TIME:
                dM.remainTime += 10;

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_good");
                break;
            case ItemType.DH.MONEY:
                dM.addCrystal(5);

                dM.soundManager.Play(SoundManager.SoundType.EFX, "item_good");
                break;
        }
        dM.setItemEffectIcon(cooltime, item.item_dh);
        if (item.item_dh != ItemType.DH.TREE) {
            Destroy(obj);
        }
    }
}