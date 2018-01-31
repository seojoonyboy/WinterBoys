using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BestHTTP;
using UnityEngine.Analytics.MiniJSON;

public class ResultModalController : MonoBehaviour {
	private SaveManager saveManager;
	private GameManager gameManager;
	private SoundManager soundManager;
    private NetworkManager networkManager;

	private UM_GameServiceManager uM_Game;
	[SerializeField] private Text distanceText;
	[SerializeField] private Text pointText;
	[SerializeField] private Button adsBtn;
	[SerializeField] private Button continueBtn;
	[SerializeField] private Button mainBtn;
	[SerializeField] private GameObject noMoneyModal;

	private GameObject currentGame;
	private SportType sport;

	private int point;
	private int extraPoint;
	private float? magnification = null;

    public GameObject rawPref;
    public GameObject[] rankPanels;
    Dictionary<string, object> dic;

    private void setManager() {
		saveManager = SaveManager.Instance;
		gameManager = GameManager.Instance;
		soundManager = SoundManager.Instance;
		uM_Game = UM_GameServiceManager.Instance;
        networkManager = NetworkManager.Instance;
    }
	
	private void Start() {
		adsBtn.onClick.AddListener(advertise);
		continueBtn.onClick.AddListener(revive);
		mainBtn.onClick.AddListener(mainLoad);
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
		setDistance(distance);
		setPoint(point, extraPoint, magnification);
		gameObject.SetActive(true);
		soundManager.Play(SoundManager.SoundType.EFX, "gameOver");

        postRecord(distance, point);

        transform.Find("CurrentScorePanel/PanelBg/DistHeader/Value").GetComponent<Text>().text = distance + " M";
        transform.Find("CurrentScorePanel/PanelBg/PointHeader/Value").GetComponent<Text>().text = point + " P";
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

    private void postRecord(float distance, int point) {
        networkManager.postRecord(postRecordCallback, sport, (int)distance, point);
    }

    private void postRecordCallback(HTTPResponse callback, SportType type) {
        DataSet dataSet = DataSet.fromJSON(callback.DataAsText);
        setResultRaw(dataSet);
    }

    private void setResultRaw(DataSet dataSet) {
        transform.Find("RankingPanel/ToggleGroup/DistRankingToggle").GetComponent<Toggle>().isOn = true;

        Distance_rank[] dist_ranks = dataSet.distance_rank;
        Transform parent = transform.Find("RankingPanel/DistRaws");

        destroyRaws(parent);

        foreach (Distance_rank data in dist_ranks) {
            GameObject raw = Instantiate(rawPref);

            raw.transform.SetParent(parent);
            raw.transform.localPosition = Vector3.zero;
            raw.transform.localScale = Vector3.one;

            Text nickname = raw.transform.Find("Panel/NickName").GetComponent<Text>();
            Text rank = raw.transform.Find("Panel/RankingNum").GetComponent<Text>();
            Text record = raw.transform.Find("Panel/Record").GetComponent<Text>();

            nickname.text = data.user.nickname;
            rank.text = data.rank + "위";
            record.text = data.distance + "M";
        }

        parent = transform.Find("RankingPanel/PointRaws");

        Point_rank[] point_ranks = dataSet.point_rank;

        destroyRaws(parent);

        foreach (Point_rank data in point_ranks) {
            GameObject raw = Instantiate(rawPref);

            raw.transform.SetParent(parent);
            raw.transform.localPosition = Vector3.zero;
            raw.transform.localScale = Vector3.one;

            Text nickname = raw.transform.Find("Panel/NickName").GetComponent<Text>();
            Text rank = raw.transform.Find("Panel/RankingNum").GetComponent<Text>();
            Text record = raw.transform.Find("Panel/Record").GetComponent<Text>();

            nickname.text = data.user.nickname;
            rank.text = data.rank + "위";
            record.text = data.distance.ToString();
        }
    }

    private void destroyRaws(Transform parent) {
        foreach(Transform raw in parent) {
            Destroy(raw.gameObject);
        }
    }

    [System.Serializable]
    public class DataSet {
        public Data data;
        public Expect_rank expect_rank;
        public Distance_rank[] distance_rank;
        public Point_rank[] point_rank;

        public static DataSet fromJSON(string json) {
            return JsonUtility.FromJson<DataSet>(json);
        }
    }

    [System.Serializable]
    public class Data {
        public int id;
        public User user;
        public int rank;
        public int distance;
        public int point;

        public static Data fromJSON(string json) {
            return JsonUtility.FromJson<Data>(json);
        }
    }

    [System.Serializable]
    public class User {
        public int id;
        public string device_id;
        public string nickname;

        public static User fromJSON(string json) {
            return JsonUtility.FromJson<User>(json);
        }
    }

    [System.Serializable]
    public class Expect_rank {
        public int distance;        //거리 기반 (변화될)순위
        public int point;           //포인트 기반 (변화될)순위

        public static Expect_rank fromJSON(string json) {
            return JsonUtility.FromJson<Expect_rank>(json);
        }
    }

    [System.Serializable]
    public class Distance_rank {
        public int id;
        public User user;
        public int rank;
        public int distance;
        public int point;

        public static Distance_rank fromJSON(string json) {
            return JsonUtility.FromJson<Distance_rank>(json);
        }
    }

    [System.Serializable]
    public class Point_rank {
        public int id;
        public User user;
        public int rank;
        public int distance;
        public int point;

        public static Point_rank fromJSON(string json) {
            return JsonUtility.FromJson<Point_rank>(json);
        }
    }
}
