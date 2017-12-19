using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics.MiniJSON;

public class SaveData {
	public int point = 99999;
	public int crystal = 10000;
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

public class SaveManager : Singleton<SaveManager> {
	protected SaveManager() { }
	private SaveData saveData;
	private void Awake() {
		DontDestroyOnLoad(gameObject);
		load();
	}

	private void load() {
        //PlayerPrefs.SetString("savefile", null);
        string saveString = PlayerPrefs.GetString("savefile");
        
		if(string.IsNullOrEmpty(saveString)) {
			saveData = new SaveData();
			return;
		}
		List<object> savefile = Json.Deserialize(saveString) as List<object>;
		if(savefile != null) {
			Debug.Log("Delete old save file");
			Debug.Log(saveString);
			saveData = new SaveData();
			return;
		}
		saveData = JsonUtility.FromJson<SaveData>(saveString);
	}

	private void save() {
		Debug.Log(JsonUtility.ToJson(saveData));
		PlayerPrefs.SetString("savefile", JsonUtility.ToJson(saveData));
	}

	public void addPoint(int point) {
		saveData.point += point;
		save();
	}

	public bool levelUpSpeed() {
		if(saveData.point < saveData.speedNeed)
			return false;
		saveData.point -= saveData.speedNeed;
		saveData.speedLv++;
		save();
		return true;
	}

	public bool levelUpControl() {
		if(saveData.point < saveData.controlNeed)
			return false;
		saveData.point -= saveData.controlNeed;
		saveData.controlLv++;
		save();
		return true;
	}

	public int getSpeedPointNeed() {
		return saveData.speedLv == 110 ? 0 : saveData.speedNeed;
	}

	public int getControlPointNeed() {
		return saveData.controlLv == 110 ? 0 : saveData.controlNeed;
	}

	public float getSpeedPercent() {
		return saveData.speedPercent;
	}

	public float getControlPercent() {
		return saveData.controlPercent;
	}

	public int getPointLeft() {
		return saveData.point;
	}

	public int getCrystalLeft() {
		return saveData.crystal;
	}

	public bool setRecord(float record, SportType sport) {
		if(saveData.maxRecord[(int)sport] < record) {
			saveData.maxRecord[(int)sport] = record;
			return true;
		}
		return false;
	}

	public float getRecord(SportType sport) {
		return saveData.maxRecord[(int)sport];
	}
}
