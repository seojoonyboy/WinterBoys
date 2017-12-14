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
    public MainSceneController mainController;
	private PointManager pointManager;

	private SportType sport = SportType.SKIJUMP;
	[SerializeField] private Sprite[] gradeSprite;
    [SerializeField] private Sprite[] characterSprite;
	[SerializeField] private Text maxScore;
	[SerializeField] public Stat speed;
	[SerializeField] public Stat control;
	[SerializeField] private Text pointLeft;
	[SerializeField] private Button startButton;
    [SerializeField] private Image character;

    public GameObject[] spines;

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

        SoundManager.Instance.Play(SoundManager.SoundType.BGM, 3);
	}

    public void OffPanel() {
        gameObject.SetActive(false);

        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 5);
    }

    public void informationIconClicked() {
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 3);
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

        character.sprite = characterSprite[GameManager.Instance.character];
        character.transform.Find("Outline/Name").GetComponent<Text>().text = mainController.charNames[GameManager.Instance.character];
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
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 2);
        if (pointManager.levelUpSpeed()) {
			Debug.Log(sport+" level up!!");
			init();
			checkButton();
			return;
		}
		Debug.Log(sport+" level up Fail");
	}

	private void levelUpControl() {
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 2);
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

    public void StartButtonClicked() {
        SoundManager.Instance.Play(SoundManager.SoundType.MAIN_SCENE, 4);
    }

    public void OnSpines() {
        if (gameObject.activeSelf) {
            foreach(GameObject obj in spines) {
                obj.SetActive(true);
            }
        }
    }

    public void OffSpines() {
        foreach(GameObject obj in spines) {
            obj.SetActive(false);
        }
    }
}
