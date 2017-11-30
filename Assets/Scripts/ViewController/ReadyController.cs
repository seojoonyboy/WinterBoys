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
		checkButton();
	}

	private void checkButton() {
		if(pointManager.getSpeedPercent() >= 1.8f)
			speed.levelUp.onClick.RemoveAllListeners();

		if(pointManager.getControlPercent() >= 1.8f)
			control.levelUp.onClick.RemoveAllListeners();
	}

	public void open(SportType sport) {
		this.sport = sport;
		gameObject.SetActive(true);
	}

	private void OnEnable() {
		init();
		setScene();
	}

	private void init() {
		maxScore.text = pointManager.getRecord(sport).ToString("#0.00");
		speed.percent.text = (100f * pointManager.getSpeedPercent() - 100f).ToString("00.0");
		speed.needPoint.text = pointManager.getSpeedPointNeed().ToString();
		setGrade(speed.grade, pointManager.getSpeedPercent());

		control.percent.text = (100f * pointManager.getControlPercent()- 100f).ToString("00.0");
		control.needPoint.text = pointManager.getControlPointNeed().ToString();
		setGrade(control.grade, pointManager.getControlPercent());

		pointLeft.text = pointManager.getPointLeft().ToString("00000");
	}

	private void setGrade(Image grade, float num) {
		if(num < 1.13f) grade.sprite = gradeSprite[0];
		else if(num < 1.25f) grade.sprite = gradeSprite[1];
		else if(num < 1.35f) grade.sprite = gradeSprite[2];
		else if(num < 1.45f) grade.sprite = gradeSprite[3];
		else if(num <= 1.5f) grade.sprite = gradeSprite[4];
		else if(num <= 1.65f)grade.sprite = gradeSprite[5];
		else if(num < 1.8f)  grade.sprite = gradeSprite[6];
		else 				 grade.sprite = gradeSprite[7];

		grade.SetNativeSize();
	}

	private void levelUpSpeed() {
		if(pointManager.levelUpSpeed()) {
			Debug.Log(sport+" level up!!");
			init();
			checkButton();
			return;
		}
		Debug.Log(sport+" level up Fail");
	}

	private void levelUpControl() {
		if (pointManager.levelUpControl()) {
			Debug.Log(sport+"level up!!");
			init();
			checkButton();
			return;
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
