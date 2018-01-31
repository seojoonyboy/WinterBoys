using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;

[System.Serializable]
public struct PointStat {
	public Image grade;
	public Text percent;
	public Text needPoint;
	public Button levelUp;
};
[System.Serializable]
public struct CharStat {
	public Image image;
	public Text name;
	public Text stat;
	public Text playTime;
	public Transform playCount;
	public Sprite fullEntry;
	public Sprite emptyEntry;
	public void setData(Sprite sprite, string name, int speed, int control) {
		image.sprite = sprite;
		this.name.text = name;
		do {
			if(speed != 0 && control != 0) {
				stat.text = string.Format("{0}+{1}%/{2}+{3}%", LocalizationManager.GetTranslation("skill_speed"), speed, 
																LocalizationManager.GetTranslation("skill_control"), control);
				break;
			}
			if(speed == 0) {
				stat.text = string.Format("{0} + {1}%", LocalizationManager.GetTranslation("skill_control"), control);
				break;
			}
			if(control == 0) {
				stat.text = string.Format("{0} + {1}%", LocalizationManager.GetTranslation("skill_speed"), speed);
				break;
			}
		} while(false);
	}
	public void setTime(int maxEntry, int entry, double leftTime) {
		Image[] images = playCount.GetComponentsInChildren<Image>();
		if(entry == maxEntry) {
			playTime.text = string.Format("{0} (00:00)", LocalizationManager.GetTranslation("ready_stamina"));
			for(int i = 0; i < images.Length; i++)
				images[i].sprite = fullEntry;
			return;
		}
		for(int i = 0; i < entry; i++)
			images[i].sprite = fullEntry;
		for(int i = entry; i < maxEntry; i++)
			images[i].sprite = emptyEntry;
		playTime.text = string.Format("{0} ({1}:{2})", LocalizationManager.GetTranslation("ready_stamina"), ((int)leftTime/60).ToString("00"), ((int)leftTime%60).ToString("00"));
	}
}

public class ReadyController : MonoBehaviour {
    public MainSceneController mainController;
	private SaveManager saveManager;
	private CharacterManager cm;

	private SportType sport = SportType.SKIJUMP;
	[SerializeField] private ResourceController topLabel;
	[SerializeField] private Sprite[] gradeSprite;
    [SerializeField] private Sprite[] characterSprite;
	[SerializeField] private CharStat charStat;
	[SerializeField] private Text maxScore;
	[SerializeField] public PointStat speed;
	[SerializeField] public PointStat control;
	[SerializeField] private Button startButton;
	[SerializeField] private GameObject tutorialModal;
	[SerializeField] private GameObject pointTutoModal;
	[SerializeField] private GameObject noStaminaModal;

    Color32 positiveButtonColor = new Color32(255, 255, 255, 255);
    Color32 negativeButtonColor = new Color32(255, 249, 23, 144);

    private void Awake() {
		saveManager = SaveManager.Instance;
		cm = CharacterManager.Instance;
		setButton();
		init();
	}

	private void setButton() {
        setButtonColor();
        speed.levelUp.onClick.AddListener(levelUpSpeed);
		control.levelUp.onClick.AddListener(levelUpControl);
		checkButton();
	}

	private void checkButton() {
		if(saveManager.getSpeedPercent() >= 1.8f)
			speed.levelUp.onClick.RemoveAllListeners();

		if(saveManager.getControlPercent() >= 1.8f)
			control.levelUp.onClick.RemoveAllListeners();
	}

	public void open(SportType sport) {
		this.sport = sport;
		gameObject.SetActive(true);
	}

	private void OnEnable() {
		init();
		setScene();
		setCharData();
        SoundManager.Instance.Play(SoundManager.SoundType.BGM, "statChange");
		checkTutorial();
        
	}

	private void checkTutorial() {
		GameManager gm = GameManager.Instance;
		if(gm.isTutorial(GameManager.tutorialEnum.POINT)) return;
		if(gm.isTutorial(GameManager.tutorialEnum.DOWNHLL)||gm.isTutorial(GameManager.tutorialEnum.SKIJUMP)) {
			pointTutoModal.SetActive(true);
			gm.tutorialDone(GameManager.tutorialEnum.POINT);
		}
		if(gm.isTutorial(GameManager.tutorialEnum.READY)) return;
        tutorialModal.SetActive(true);
		gm.tutorialDone(GameManager.tutorialEnum.READY);
	}

    public void OffPanel() {
        gameObject.SetActive(false);

        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "closeBtn");
    }

    public void informationIconClicked() {
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "showInfo");
    }

    private void init() {
		maxScore.text = saveManager.getRecord(sport).ToString("#0");
		float speedPercent = saveManager.getSpeedPercent()-cm.getSpeedPercent;
		speed.percent.text = (100f * speedPercent - 100f).ToString("00.0");
		speed.needPoint.text = saveManager.getSpeedPointNeed().ToString();
		setGrade(speed.grade, speedPercent);

		float controlPercent = saveManager.getControlPercent() - cm.getControlPercent;
		control.percent.text = (100f * controlPercent - 100f).ToString("00.0");
		control.needPoint.text = saveManager.getControlPointNeed().ToString();
		setGrade(control.grade, controlPercent);

		topLabel.setData();
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
        setButtonColor();
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "statChange");
        if (saveManager.levelUpSpeed()) {
			Debug.Log(sport+" level up!!");
			init();
			checkButton();
			return;
		}
		Debug.Log(sport+" level up Fail");
	}

	private void levelUpControl() {
        setButtonColor();
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "statChange");
        if (saveManager.levelUpControl()) {
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
			startButton.onClick.AddListener(() => startGame("SkiJump", GameManager.tutorialEnum.SKIJUMP));
			buttonText.text = "스키점프 시작!";
			break;
			case SportType.SKELETON :
			startButton.onClick.AddListener(() => startGame("Skeleton", GameManager.tutorialEnum.SKELETON));
			buttonText.text = "스켈레톤 시작!";
			break;
			case SportType.DOWNHILL :
			startButton.onClick.AddListener(() => startGame("DownHill", GameManager.tutorialEnum.DOWNHLL));
			buttonText.text = "다운힐 시작!";
			break;
		}
    }

	private void startGame(string sceneName, GameManager.tutorialEnum tutorial) {
		if(!cm.playGame()) {
			noStaminaModal.SetActive(true);
			return;
		}
		bool isTutorial = GameManager.Instance.isTutorial(tutorial);
		if(!isTutorial) {
			GameManager.Instance.tutorialSports = tutorial;
			SceneManager.LoadScene("Tutorial");
			return;
		}
		SceneManager.LoadScene(sceneName);
	}

    public void StartButtonClicked() {
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "gameStartBtn");
    }

	public void setCharData() {
		int num = cm.currentCharacter;
		charStat.setData(characterSprite[num], cm.getName(num), cm.getSpeed(num), cm.getControl(num));
		PlayerPrefs.SetInt("character", num);
		init();
	}

    private void setButtonColor() {
        if (saveManager.levelUpSpeed()) {
            speed.levelUp.GetComponent<Image>().color = positiveButtonColor;
        }
        else {
            speed.levelUp.GetComponent<Image>().color = negativeButtonColor;
        }

        if (saveManager.levelUpControl()) {
            control.levelUp.GetComponent<Image>().color = positiveButtonColor;
        }
        else {
            control.levelUp.GetComponent<Image>().color = negativeButtonColor;
        }
    }

	private void FixedUpdate() {
		int num = cm.currentCharacter;
		charStat.setTime(cm.getMaxEntry(num), cm.getCurrentEntry(num), cm.getLeftTime(num));
	}
}
