using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkeletonManager : MonoBehaviour {
    [SerializeField] private GameManager gm;
    [SerializeField] private Transform background;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject warningImg;
    [SerializeField] private GameObject gameoverModal;
    [SerializeField] private Text speedUI;
    [SerializeField] private Text distanceUI;
    [SerializeField] private Text leftTimeUI;
    [SerializeField] private GameObject showAddTimeUI;
    private float dangerTime;
    private float currentSpeed = 0f;
    private float maxSpeed;
    private float totalDistance = 0f;
    private float leftTime;
    private float distanceBonusTime = 0f;
    bool showTime = true;
    int showCount = 0;

    void Start() {
        gm = GameManager.Instance;
        maxSpeed = gm.skeleton_stats[0];
        leftTime = gm.startTime;
    }
    public void mainLoad() {
        SceneManager.LoadScene("Main");
    }

    void FixedUpdate() {
        if(checkRotated()) {
            dangerTime += Time.fixedDeltaTime;
            addSpeed(-0.2f);
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
            gameOver();
        }
    }

    private bool checkRotated() {
        float rotated = (player.eulerAngles.z - background.eulerAngles.z);
        if(rotated > 180f) rotated -= 360f;
        return (rotated > gm.skeleton_dangers[1] || rotated < -gm.skeleton_dangers[1]);
    }

    private bool checkFalldown() {
        if(dangerTime >= gm.skeleton_dangers[2]) return true;
        return false;
    }

    private void addSpeed(float force) {
        currentSpeed += force;
        if(currentSpeed < 0f) currentSpeed = 0f;
        if(currentSpeed > maxSpeed) currentSpeed = maxSpeed;
        speedUI.text = currentSpeed.ToString("##.0");
        background.GetComponent<Animator>().speed = currentSpeed * 0.03f;
    }

    private void addDistance(float time) {
        float move = (currentSpeed * time * 0.28f);//0.28 시속 보정
        totalDistance += move;
        distanceBonusTime += move;
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

    private void gameOver() {
        gameoverModal.SetActive(true);
        this.enabled = false;
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
}
