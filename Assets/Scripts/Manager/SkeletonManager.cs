using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameEvents;

public class SkeletonManager : MonoBehaviour {
    private GameManager gm;
    private PointManager pm;
    private EventManager em;
    [SerializeField] private Transform background;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject warningImg;
    [SerializeField] private GameObject gameoverModal;
    [SerializeField] private Text speedUI;
    [SerializeField] private Text distanceUI;
    [SerializeField] private Text leftTimeUI;
    [SerializeField] private GameObject showAddTimeUI;
    [SerializeField] private Skeleton_TrackController track;
    [SerializeField] private Button replayBtn;
    [SerializeField] private ItemGenerator itemGenerator;
    private float dangerTime;
    private float currentSpeed = 0f;
    private float maxSpeed;
    private float totalDistance = 0f;
    private float leftTime;
    private float distanceBonusTime = 0f;
    private float turnTime = 0f;
    private float turnWhen = 200f;
    private float turnCount = 10f;
    private bool showTime = true;
    private int showCount = 0;
    public enum arrow {FRONT, LEFT, RIGHT};
    public arrow direction = arrow.FRONT;

    private delegate void skeletonUpdate(float time);
    private skeletonUpdate stateUpdate;

    private void Awake() {
        gm = GameManager.Instance;
        pm = PointManager.Instance;
        em = EventManager.Instance;
        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void OnDestroy() {
        Screen.orientation = ScreenOrientation.Portrait;
        removeEvent();
    }

    private void Start() {
        replayBtn.onClick.AddListener(replay);
        maxSpeed = gm.skeleton_stats[0] * pm.getSpeedPercent();
        leftTime = gm.startTime;
        addEvent();
        stateUpdate = riseUpdate;
        stateUpdate += track.riseUpdate;
    }

    private void addEvent() {
        em.AddListener<Skeleton_Fall>(playerFall);
        em.AddListener<Skeleton_Rise>(playerRise);
    }

    private void removeEvent() {
        em.RemoveListener<Skeleton_Fall>(playerFall);
        em.RemoveListener<Skeleton_Rise>(playerRise);
    }

    private void playerFall(Skeleton_Fall e) {
        stateUpdate = fallUpdate;
        stateUpdate += track.fallUpdate;
        player.GetComponent<Skeleton_PlayerController>().enabled = false;
        dangerTime = 0f;
    }

    private void playerRise(Skeleton_Rise e) {
        stateUpdate = riseUpdate;
        stateUpdate += track.riseUpdate;
        player.GetComponent<Skeleton_PlayerController>().enabled = true;
        dangerTime = 0f;
    }

    private void FixedUpdate() {
        stateUpdate(Time.fixedDeltaTime);
    }

    private void riseUpdate(float time) {
        if(checkRotated()) {
            dangerTime += Time.fixedDeltaTime;
            addSpeed(0.05f);
            warningImg.SetActive(true);
        }
        else {
            dangerTime = 0f;
            addSpeed(0.1f);
            warningImg.SetActive(false);
        }

        addDistance(Time.fixedDeltaTime);
        checkTime(Time.fixedDeltaTime);

        if(checkFalldown()) {
            em.TriggerEvent(new Skeleton_Fall());
        }

        if(checkFallOut()) {
            gameOver();
        }
    }

    private void fallUpdate(float time) {
        addSpeed(-1.0f);
        addDistance(Time.fixedDeltaTime);
        checkTime(Time.fixedDeltaTime);
        checkFallTime(Time.fixedDeltaTime);
    }

    private bool checkRotated() {
        float rotated = player.eulerAngles.z;
        if(rotated > 180f) rotated -= 360f;
        return (rotated > (currectDirection() + gm.skeleton_dangers[1]) || rotated < (currectDirection() - gm.skeleton_dangers[1]));
    }

    private float currectDirection() {
        if(direction == arrow.LEFT) return -45f;
        if(direction == arrow.RIGHT) return 45f;
        return 0f;
    }

    private bool checkFalldown() {
        if(dangerTime >= gm.skeleton_dangers[2]) return true;
        return false;
    }

    private bool checkFallOut() {
        float rotated = player.eulerAngles.z;
        if(rotated > 180f) rotated -= 360f;
        return (rotated > 100f || rotated < -100f);
    }

    private void addSpeed(float force) {
        currentSpeed += force;
        if(currentSpeed < 0f) currentSpeed = 0f;
        if(currentSpeed > maxSpeed) currentSpeed = maxSpeed;
        speedUI.text = currentSpeed.ToString("#0.0");
        track.setSpeed(currentSpeed * 0.03f);
    }

    private void addDistance(float time) {
        float move = (currentSpeed * time * 0.28f);//0.28 시속 보정
        totalDistance += move;
        distanceBonusTime += move;
        turnTime += move;
        checkTurn();
        distanceUI.text = totalDistance.ToString("#0");
    }

    private void checkTime(float time) {
        leftTime -= time;
        leftTimeUI.text = leftTime.ToString("0");
        if(distanceBonusTime >= gm.skeleton_bonus_times[0]) {
            distanceBonusTime = 0f;
            leftTime += gm.skeleton_bonus_times[1];
            InvokeRepeating("showAddTime",0f, 0.5f);
        }
        if(leftTime <= 0f) gameOver();
    }

    private void checkFallTime(float time) {
        dangerTime += time;
        if(dangerTime > gm.skeleton_dangers[0])
            em.TriggerEvent(new Skeleton_Rise());
    }

    private void checkTurn() {
        if(turnTime >= turnWhen) {
            turnTime -= turnWhen;
            if(turnWhen > 30f) 
                turnWhen -= turnCount;
            track.triggerTurn();
            itemGenerator.Generate(SportType.SKELETON);
        }
    }

    private void gameOver() {
        gameoverModal.SetActive(true);
        track.setSpeed(0);
        this.enabled = false;
        setGameOverUI();
    }

    private void setGameOverUI() {
        Text distance = gameoverModal.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        Text point = gameoverModal.transform.GetChild(0).GetChild(2).GetComponent<Text>();

        int _point = (int)(totalDistance / gm.skeleton_point[1]);

        distance.text = string.Format("이동 거리 : {0}", totalDistance.ToString("##.0"));
        point.text = string.Format("포인트 : {0}", _point);
        if(replayBtn == null) return;
        int randNum = Random.Range(0, 100);
        if(randNum < 15) {
            replayBtn.gameObject.SetActive(true);
        }
        else {
            replayBtn.gameObject.SetActive(false);
        }
    }

    public void mainLoad() {
        int _point = (int)(totalDistance / gm.skeleton_point[1]);

        if(pm.setRecord(totalDistance, SportType.SKELETON)) {
            UM_GameServiceManager.Instance.SubmitScore("Skeleton", (long)totalDistance);
        }
        pm.addPoint(_point);
        SceneManager.LoadScene("Main");
    }

    private void showAddTime() {
        showAddTimeUI.transform.GetComponentInChildren<Text>().text = gm.skeleton_bonus_times[1].ToString();
        showAddTimeUI.SetActive(showTime);
        showTime = !showTime;
        showCount++;
        if(showCount == 10) {
            showCount = 0;
            CancelInvoke("showAddTime");
        }
    }

    private void replay() {
        distanceBonusTime = 0f;
        leftTime = gm.startTime;
        dangerTime = 0f;
        currentSpeed = 0f;

        gameoverModal.SetActive(false);
        this.enabled = true;
        Destroy(replayBtn);
        replayBtn.gameObject.SetActive(false);
    }
}
