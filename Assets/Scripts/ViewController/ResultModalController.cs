using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultModalController : MonoBehaviour {
	private SaveManager saveManager;
	private GameManager gameManager;
	private SoundManager soundManager;
	private UM_GameServiceManager uM_Game;
	[SerializeField] private Text timeText;
	[SerializeField] private Text distanceText;
	[SerializeField] private Text maxComboText;
	[SerializeField] private Text pointText;
	[SerializeField] private Button adsBtn;
	[SerializeField] private Button continueBtn;
	[SerializeField] private Button mainBtn;
	[SerializeField] private Button rankBtn;
	[SerializeField] private GameObject noMoneyModal;

	private GameObject currentGame;
	private SportType sport;

	private int point;
	private int extraPoint;
	private float? magnification = null;

	private void setManager() {
		saveManager = SaveManager.Instance;
		gameManager = GameManager.Instance;
		soundManager = SoundManager.Instance;
		uM_Game = UM_GameServiceManager.Instance;
	}
	
	private void Start() {
		adsBtn.onClick.AddListener(advertise);
		continueBtn.onClick.AddListener(revive);
		mainBtn.onClick.AddListener(mainLoad);
		rankBtn.onClick.AddListener(rankLoad);
		showAdsBtn();
	}

	private void showAdsBtn() {
		int randNum = Random.Range(0, 100);
        adsBtn.gameObject.SetActive(randNum < 30 ? true : false);
	}
/// <summary>
/// 현재 하는 게임을 기록하는 함수
/// </summary>
/// <param name="game">해당 스포츠 매니저의 게임 오브젝트</param>
/// <param name="sport">해당 스포츠의 ENUM</param>
	public void setGame(GameObject game, SportType sport) {
		setManager();
		
		currentGame = game;
		this.sport = sport;
	}
/// <summary>
/// 게임 오버시 데이터 기록 및 표시
/// </summary>
/// <param name="time">시간(초단위)</param>
/// <param name="distance">거리</param>
/// <param name="point">포인트</param>
/// <param name="extraPoint">추가 포인트</param>
/// <param name="maxCombo">최대 콤보(다운힐 외 null)</param>
/// <param name="magnification">배율(스키 점프 외 null)</param>
	public void setData(float time, float distance, int point, int extraPoint, int? maxCombo, float? magnification) {
		setTime(sport, time);
		setMaxCombo(maxCombo);
		setDistance(distance);
		setPoint(point, extraPoint, magnification);
		gameObject.SetActive(true);
		soundManager.Play(SoundManager.SoundType.EFX, "gameOver");
	}

    private void setTime(SportType sport, float time) {
        if(sport == SportType.SKIJUMP) timeText.text = string.Format("{0}초", time.ToString("0"));
		else timeText.text = string.Format("{0} : {1}", ((int)time / 60).ToString("00"), ((int)time % 60).ToString("00"));
    }

    private void setMaxCombo(int? maxCombo) {
		if(maxComboText == null) return;
		if(maxCombo.HasValue) maxComboText.text = maxCombo.Value.ToString();
		else Destroy(maxComboText.transform.parent.gameObject);
    }

	private void setDistance(float distance) {
		distanceText.text = string.Format("{0} M", distance.ToString("##,0"));
        if(saveManager.setRecord(distance, sport)) {
            UM_GameServiceManager.Instance.SubmitScore(getSportString(), (long)distance);
        }
    }

    private void setPoint(int point, int extraPoint, float? magnification) {
		this.point = point;
		this.extraPoint = extraPoint;
		if(magnification.HasValue) {
			this.magnification = magnification.Value;
			pointText.resizeTextForBestFit = true;
		}
		string magniText = magnification.HasValue ? string.Format("(배율:x{0})", magnification.Value): "";
        pointText.text = string.Format("{0} + {1}{2}", point, extraPoint, magniText);
    }

	private void advertise() {
        UnityAdsHelper ads = UnityAdsHelper.Instance;
        ads.onResultCallback += adsCallBack;
        ads.ShowRewardedAd();
    }

	private void adsCallBack(UnityEngine.Advertisements.ShowResult result) {
        UnityAdsHelper.Instance.onResultCallback -= adsCallBack;
        if(result != UnityEngine.Advertisements.ShowResult.Finished) return;
        Destroy(adsBtn);
        Destroy(continueBtn);
        adsBtn.gameObject.SetActive(false);
        if(continueBtn != null) continueBtn.gameObject.SetActive(false);
        setPoint(point * 2, extraPoint * 2, magnification);
    }

	private void revive() {
		soundManager.Play(SoundManager.SoundType.EFX, "resumeBtn");
		//돈 부족시 코드 필요 .
		if(SaveManager.Instance.getCrystalLeft() < 50) {
			noMoneyModal.SetActive(true);
			return;
		}
		SaveManager.Instance.useCrystal(50);
		currentGame.SendMessage(getSportRevive(), SendMessageOptions.DontRequireReceiver);
		Destroy(continueBtn);
        continueBtn.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	private void mainLoad() {
		saveManager.addPoint(point+extraPoint);
		soundManager.Play(SoundManager.SoundType.EFX, "returnMain");
        gameManager.LoadSceneFromIngame("Main", sport);
	}

	private void rankLoad() {
		soundManager.Play(SoundManager.SoundType.EFX, "rankingShow");
		uM_Game.ShowLeaderBoardUI(getSportString());
	}

	private string getSportString() {
		switch(sport) {
			case SportType.DOWNHILL : return "DownHill";
			case SportType.SKELETON : return "Skeleton";
			case SportType.SKIJUMP : return "SkiJump";
		}
		return "Downhill";
	}

	private string getSportRevive() {
		switch(sport) {
			case SportType.DOWNHILL : return "resume";
			case SportType.SKELETON : return "revive";
			case SportType.SKIJUMP : return "resumneButtonPressed";
		}
		return "revive";
	}
}
