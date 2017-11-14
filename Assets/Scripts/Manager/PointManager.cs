using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.MiniJSON;

public class PointData {
	public int point = 999999;
	public int speedLv = 0;
	public int controlLv = 0;
	public float[] maxRecord = {0f, 0f, 0f};

	public int speedNeed {get {return pointNeed(speedLv);}}

	public int controlNeed {get {return pointNeed(controlLv);}}
	public float speedPercent {get{return 1.0f + speedLv * 0.005f;}}
	public float controlPercent {get{return 1.0f + controlLv * 0.005f;}}

	private int pointNeed(int level) {
		int switchLv = level / 10;
		switch(switchLv) {
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
	protected PointManager() { }
	private PointData pointData;
	private void Awake() {
		DontDestroyOnLoad(gameObject);
		load();
	}

	private void load() {
		string saveString = PlayerPrefs.GetString("savefile");
		if(string.IsNullOrEmpty(saveString)) {
			pointData = new PointData();
			return;
		}
		List<object> savefile = Json.Deserialize(saveString) as List<object>;
		if(savefile != null) {
			Debug.Log("Delete old save file");
			Debug.Log(saveString);
			pointData = new PointData();
			return;
		}
		pointData = JsonUtility.FromJson<PointData>(saveString);
	}

	private void save() {
		Debug.Log(JsonUtility.ToJson(pointData));
		PlayerPrefs.SetString("savefile", JsonUtility.ToJson(pointData));
	}

	public void addPoint(int point) {
		pointData.point += point;
		save();
	}

	public bool levelUpSpeed() {
		if(pointData.point < pointData.speedNeed)
			return false;
		pointData.point -= pointData.speedNeed;
		pointData.speedLv++;
		save();
		return true;
	}

	public bool levelUpControl() {
		if(pointData.point < pointData.controlNeed)
			return false;
		pointData.point -= pointData.controlNeed;
		pointData.controlLv++;
		save();
		return true;
	}

	public int getSpeedPointNeed() {
		return pointData.speedNeed;
	}

	public int getControlPointNeed() {
		return pointData.controlNeed;
	}

	public float getSpeedPercent() {
		return pointData.speedPercent;
	}

	public float getControlPercent() {
		return pointData.controlPercent;
	}

	public int getPointLeft() {
		return pointData.point;
	}

	public bool setRecord(float record, SportType sport) {
		if(pointData.maxRecord[(int)sport] < record) {
			pointData.maxRecord[(int)sport] = record;
			return true;
		}
		return false;
	}

	public float getRecord(SportType sport) {
		return pointData.maxRecord[(int)sport];
	}
}
