﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameEvents;

public class SkeletonManager : MonoBehaviour {
    private GameManager gm;
    private SaveManager pm;
    private EventManager em;
    private SoundManager sm;
    private AudioSource smSource;
    [SerializeField] private ResultModalController resultModal;
    [SerializeField] private Transform background;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject warningImg;
    [SerializeField] private Text speedUI;
    [SerializeField] private Text distanceUI;
    [SerializeField] private Text leftTimeUI;
    [SerializeField] private GameObject showAddTimeUI;
    [SerializeField] private Skeleton_TrackController track;
    //[SerializeField] private Button replayBtn;
    //[SerializeField] private Button adsBtn;
    [SerializeField] private ItemGenerator itemGenerator;
    private float dangerTime;
    private float dangerAngle;
    private float currentSpeed = 0f;
    private float maxSpeed;
    private float totalDistance = 0f;
    private float leftTime;
    private float totalTime;
    private float distanceBonusTime = 0f;
    private float turnDistance = 0f;
    private float turnWhen = 200f;
    private float itemDistance = 0f;
    private float itemWhen = 50f;
    private bool showTime = true;
    private int showCount = 0;
    public enum arrow {FRONT, LEFT, RIGHT};
    public arrow direction = arrow.FRONT;

    private delegate void skeletonUpdate(float time);
    private skeletonUpdate stateUpdate;
    private skeletonUpdate itemUpdate;
    private int extraPoint = 0;
    private float[] coolTimes = {0f,0f,0f,0f,0f};

    private void Awake() {
        gm = GameManager.Instance;
        pm = SaveManager.Instance;
        em = EventManager.Instance;
        sm = SoundManager.Instance;
        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void OnDestroy() {
        Screen.orientation = ScreenOrientation.Portrait;
        removeEvent();
    }

    private void Start() {
        smSource = sm.transform.GetChild(1).GetComponent<AudioSource>();
        sm.Play(SoundManager.SoundType.BGM, "st");
        maxSpeed = gm.skeleton_stats[0] * pm.getSpeedPercent();
        dangerAngle = gm.skeleton_dangers[1];
        leftTime = 40f;
        totalTime = 0f;
        addEvent();
        stateUpdate = riseUpdate;
        stateUpdate += track.riseUpdate;
    }
////////////////////////////////// 이벤트 구간 /////////////////////////////////////////////////
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
////////////////////////////////// 규칙 구간 /////////////////////////////////////////////////
    private void FixedUpdate() {
        stateUpdate(Time.fixedDeltaTime);
        if(itemUpdate == null) return;
        itemUpdate(Time.fixedDeltaTime);
    }

    private void riseUpdate(float time) {
        if(checkRotated()) {
            dangerTime += Time.fixedDeltaTime;
            addSpeed(maxSpeed * 0.012f);
            warningImg.SetActive(true);

            if(!smSource.isPlaying) sm.Play(SoundManager.SoundType.EFX, "st_warning");
        }
        else {
            dangerTime = 0f;
            addSpeed(maxSpeed * 0.012f);
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
        return (rotated > (currectDirection() + dangerAngle) || rotated < (currectDirection() - dangerAngle));
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
        return (rotated > 90f || rotated < -90f);
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
        turnDistance += move;
        itemDistance += move;
        checkTurn();
        checkItem();
        distanceUI.text = totalDistance.ToString("#0");
    }

    private void checkTime(float time) {
        leftTime -= time;
        totalTime += time;
        leftTimeUI.text = leftTime.ToString("0");
        if(distanceBonusTime >= gm.skeleton_bonus_times[0]) {
            distanceBonusTime = 0f;
            leftTime += gm.skeleton_bonus_times[1];
            showAddTimeUI.transform.GetComponentInChildren<Text>().text = gm.skeleton_bonus_times[1].ToString();
            InvokeRepeating("showAddTime",0f, 0.5f);
            sm.Play(SoundManager.SoundType.EFX, "timeAdd");
        }
        if(leftTime <= 0f) gameOver();
    }

    private void checkFallTime(float time) {
        dangerTime += time;
        if(dangerTime > gm.skeleton_dangers[0])
            em.TriggerEvent(new Skeleton_Rise());
    }

    private void checkTurn() {
        if(turnDistance >= turnWhen) {
            turnDistance -= turnWhen;

            track.triggerTurn();
            sm.Play(SoundManager.SoundType.EFX, "st_conering");
            if(totalDistance >= 600f) {
                turnWhen = 150f;
            }
            else if(totalDistance >= 1500f) {
                turnWhen = 100f;
            }
        }
        if(smSource.isPlaying) return;
        sm.Play(SoundManager.SoundType.EFX, "st_move");
    }

    private void checkItem() {
        if(itemDistance >= itemWhen) {
            itemDistance -= itemWhen;
            itemWhen = 80f;
            itemGenerator.Generate(SportType.SKELETON);
            itemGenerator.Generate(SportType.SKELETON);

            if(totalDistance >= 350f) {
                itemWhen = 60f;
                itemGenerator.Generate(SportType.SKELETON);
            } else if (totalDistance >= 2000f) {
                itemWhen = 40f;
                itemGenerator.Generate(SportType.SKELETON);
                itemGenerator.Generate(SportType.SKELETON);
            }
        }
    }

    private void showAddTime() {
        showAddTimeUI.SetActive(showTime);
        showTime = !showTime;
        showCount++;
        if(showCount == 10) {
            showCount = 0;
            CancelInvoke("showAddTime");
        }
    }
////////////////////////////////// 아이템 구간 /////////////////////////////////////////////////
    private void getItem(ItemType.ST st) {
        switch(st) {
            case ItemType.ST.POINT :
            extraPoint += 10;
            break;
            case ItemType.ST.BOOST :
            maxSpeed = 1000f;
            addSpeed(1000f);
            itemUpdate += boostTime;
            break;
            case ItemType.ST.ICE :
            maxSpeed = gm.skeleton_stats[0] * pm.getSpeedPercent() * 0.75f;
            itemUpdate += iceTime;
            break;
            case ItemType.ST.BUGS :
            player.SendMessage("reverseItem", true, SendMessageOptions.DontRequireReceiver);
            itemUpdate += bugTime;
            break;
            case ItemType.ST.BOND :
            dangerAngle *= 1.2f;
            itemUpdate += bondTime;
            break;
            case ItemType.ST.OIL :
            dangerAngle *= 0.75f;
            itemUpdate += oilTime;
            break;
            case ItemType.ST.MONEY :
            pm.addCrystal(5);
            break;
            case ItemType.ST.WATCH :
            leftTime += 15f;
            showAddTimeUI.transform.GetComponentInChildren<Text>().text = 15.ToString();
            InvokeRepeating("showAddTime",0f, 0.5f);
            sm.Play(SoundManager.SoundType.EFX, "timeAdd");
            break;
        }
    }

    private void boostTime(float time) {
        coolTimes[0] += time;
        if(coolTimes[0] < 3f) return;
        coolTimes[0] = 0f;
        itemUpdate -= boostTime;
        maxSpeed = gm.skeleton_stats[0] * pm.getSpeedPercent();
    }

    private void iceTime(float time) {
        coolTimes[1] += time;
        if(coolTimes[1] < 7f) return;
        coolTimes[1] = 0f;
        itemUpdate -= iceTime;
        maxSpeed = gm.skeleton_stats[0] * pm.getSpeedPercent();
    }

    private void bugTime(float time) {
        coolTimes[2] += time;
        if(coolTimes[2] < 7f) return;
        coolTimes[2] = 0f;
        itemUpdate -= bugTime;
        player.SendMessage("reverseItem", false, SendMessageOptions.DontRequireReceiver);
    }

    private void bondTime(float time) {
        coolTimes[3] += time;
        if(coolTimes[3] < 10f) return;
        coolTimes[3] = 0f;
        itemUpdate -= bondTime;
        dangerAngle = gm.skeleton_dangers[1];
    }
    private void oilTime(float time) {
        coolTimes[4] += time;
        if(coolTimes[4] < 10f) return;
        coolTimes[4] = 0f;
        itemUpdate -= oilTime;
        dangerAngle = gm.skeleton_dangers[1];
    }
////////////////////////////////// 게임오버 구간 /////////////////////////////////////////////////
    private void gameOver() {
        track.setSpeed(0);
        this.enabled = false;
        resultModal.setGame(gameObject, SportType.SKELETON);
        resultModal.setData(totalTime, totalDistance, (int)(totalDistance / gm.skeleton_point[1] + gm.skeleton_point[0]), extraPoint, null, null);
    }

    private void revive() {
        distanceBonusTime = 0f;
        leftTime = 40f;
        dangerTime = 0f;
        currentSpeed = 0f;
        this.enabled = true;
    }
}
