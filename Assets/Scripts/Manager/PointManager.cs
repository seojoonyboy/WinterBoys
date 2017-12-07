using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.MiniJSON;

public class PointData {
	public int point = 99999;
	public int speedLv = 0;
	public int controlLv = 0;
	public float[] maxRecord = {0f, 0f, 0f};

	public int speedNeed {get {return pointNeed(speedLv);}}

	public int controlNeed {get {return pointNeed(controlLv);}}
	public float speedPercent {get{return 1.0f + speedLv * 0.01f - (speedLv > 50 ? (speedLv - 50) * 0.005f : 0f);}}
	public float controlPercent {get{return 1.0f + controlLv * 0.01f - (controlLv > 50 ? (controlLv - 50) * 0.005f : 0f);}}

	private int pointNeed(int level) {
		int switchLv = level / 10;
		if(level <= 15)		return 20 + (30 * level);
		if(level <= 30)		return 20 + (30 * 15) + (60 * (level - 15));
		if(level <= 50)		return 20 + (30 * 15) + (60 * 15) + (120 * (level - 30));
		return 20 + (30 * 15) + (60 * 15) + (120 * 20) + (80 * (level - 50));
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
        //PlayerPrefs.SetString("savefile", null);
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
		return pointData.speedLv == 110 ? 0 : pointData.speedNeed;
	}

	public int getControlPointNeed() {
		return pointData.controlLv == 110 ? 0 : pointData.controlNeed;
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
