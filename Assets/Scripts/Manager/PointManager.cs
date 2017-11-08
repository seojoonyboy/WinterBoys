using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.MiniJSON;

public enum SportType {
	SKIJUMP,
	SKELETON,
	DOWNHILL
}

public class PointData {
	public int point = 0;
	public int speedLv = 0;
	public int controlLv = 0;

	public int speedNeed {get {return pointNeed(speedLv);}}

	public int controlNeed {
		get {return pointNeed(controlLv);}}
	public float speedPercent {get{return 100f + speedLv * 0.5f;}}
	public float controlPercent {get{return 100f + controlLv * 0.5f;}}

	private int pointNeed(int level) {
		level /= 10;
		switch(level) {
			case 0 : case 1 :
			return (int)(50f * Mathf.Pow(1.2f, level));
			case 2 : case 3 :
			return (int)(50f * Mathf.Pow(1.2f, 20) * Mathf.Pow(1.1f, level - 20));
			case 4 : case 5 :
			return (int)(50f * Mathf.Pow(1.2f, 20) * Mathf.Pow(1.1f, 20) * Mathf.Pow(1.07f, level - 40));
			case 6 : case 7 :
			return (int)(50f * Mathf.Pow(1.2f, 20) * Mathf.Pow(1.1f, 20) * Mathf.Pow(1.07f, 20) * Mathf.Pow(1.04f, level - 60));
			default :
			return (int)(50f * Mathf.Pow(1.2f, 20) * Mathf.Pow(1.1f, 20) * Mathf.Pow(1.07f, 20) * Mathf.Pow(1.04f, 20) * Mathf.Pow(1.01f, level - 80));
		}
	}

}

public class PointManager : Singleton<PointManager> {
	private PointData[] pointData;
	private void Awake() {
		load();
	}

	private void load() {
		pointData = new PointData[(int)SportType.DOWNHILL + 1];
		string saveString = PlayerPrefs.GetString("savefile");
		if(string.IsNullOrEmpty(saveString)) {
			for(int i = 0; i <= (int)SportType.DOWNHILL ; i++)
				pointData[i] = new PointData();
			return;
		}
		List<object> savefile = Json.Deserialize(saveString) as List<object>;
		for(int i = 0; i <= (int)SportType.DOWNHILL; i++) {
			pointData[i] = JsonUtility.FromJson<PointData>((string)savefile[i]);
		}
		save();
	}

	private void save() {
		List<object> savefile = new List<object>();
		for(int i = 0; i <= (int)SportType.DOWNHILL ; i++) {
			pointData[i] = new PointData();
			savefile.Add(JsonUtility.ToJson(pointData[i]));
		}
		PlayerPrefs.SetString("savefile", Json.Serialize(savefile));
	}

	public void addPoint(int point, SportType sport) {
		pointData[(int)sport].point += point;
		save();
	}

	public bool levelUpSpeed(SportType sport) {
		if(pointData[(int)sport].point < pointData[(int)sport].speedNeed)
			return false;
		pointData[(int)sport].point -= pointData[(int)sport].speedNeed;
		pointData[(int)sport].speedLv++;
		save();
		return true;
	}

	public bool levelUpControl(SportType sport) {
		if(pointData[(int)sport].point < pointData[(int)sport].controlNeed)
			return false;
		pointData[(int)sport].point -= pointData[(int)sport].controlNeed;
		pointData[(int)sport].controlLv++;
		save();
		return true;
	}

	public int getSpeedPointNeed(SportType sport) {
		return pointData[(int)sport].speedNeed;
	}

	public int getControlPointNeed(SportType sport) {
		return pointData[(int)sport].controlNeed;
	}

	public float getSpeedPercent(SportType sport) {
		return pointData[(int)sport].speedPercent;
	}

	public float getControlPercent(SportType sport) {
		return pointData[(int)sport].controlPercent;
	}
}
