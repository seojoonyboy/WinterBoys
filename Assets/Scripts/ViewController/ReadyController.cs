using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Stat {
	public Image grade;
	public Text percent;
	public Text needPoint;
	public Button levelUp;
};

public class ReadyController : MonoBehaviour {
	private PointManager pointManager;
	private SportType sport = SportType.SKIJUMP;
	[SerializeField] private Sprite[] gradeSprite;
	[SerializeField] private Text maxScore;
	[SerializeField] public Stat speed;
	[SerializeField] public Stat control;
	[SerializeField] private Text pointLeft;
	[SerializeField] private Button startButton;

	private void Awake() {
		pointManager = PointManager.Instance;
		setButton();
		init();
	}

	private void setButton() {
		speed.levelUp.onClick.AddListener(levelUpSpeed);
		control.levelUp.onClick.AddListener(levelUpControl);
	}

	public void open(SportType sport) {
		this.sport = sport;
		init();
		setScene();
		gameObject.SetActive(true);
	}

	private void init() {
		speed.percent.text = pointManager.getSpeedPercent(sport).ToString("00.#");
		speed.needPoint.text = pointManager.getSpeedPointNeed(sport).ToString();
		setGrade(speed.grade, pointManager.getSpeedPercent(sport));

		control.percent.text = pointManager.getControlPercent(sport).ToString("00.#");
		control.needPoint.text = pointManager.getControlPointNeed(sport).ToString();
		setGrade(control.grade, pointManager.getControlPercent(sport));

		pointLeft.text = pointManager.getPointLeft(sport).ToString("00000");
	}

	private void setGrade(Image grade, float num) {
		if(num < 13f) grade.sprite = gradeSprite[3];
		else if(num < 25f) grade.sprite = gradeSprite[2];
		else if(num < 35f) grade.sprite = gradeSprite[1];
		else grade.sprite = gradeSprite[0];

		grade.SetNativeSize();
	}

	private void levelUpSpeed() {
		if(pointManager.levelUpSpeed(sport)) {
			Debug.Log(sport+" level up!!");
			init();
			return;
		}
		Debug.Log(sport+" level up Fail");
	}

	private void levelUpControl() {
		if (pointManager.levelUpControl(sport)) {
			Debug.Log(sport+"level up!!");
			init();
		}
		Debug.Log(sport+" level up Fail");
	}

	private void setScene() {
		startButton.onClick.RemoveAllListeners();
		Text buttonText = startButton.GetComponentInChildren<Text>();
		switch(sport) {
			case SportType.SKIJUMP :
			startButton.onClick.AddListener(() => SceneManager.LoadScene("SkiJump"));
			buttonText.text = "스키점프 시작!";
			break;
			case SportType.SKELETON :
			startButton.onClick.AddListener(() => SceneManager.LoadScene("Skeleton"));
			buttonText.text = "스켈레톤 시작!";
			break;
			case SportType.DOWNHILL :
			startButton.onClick.AddListener(() => SceneManager.LoadScene("DownHill"));
			buttonText.text = "다운힐 시작!";
			break;
		}
	}
}
