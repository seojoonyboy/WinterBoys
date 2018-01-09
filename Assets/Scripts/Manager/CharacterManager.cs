using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SportPlayer {
    [SerializeField] private string name;
    public virtual string Name {get{return name;}}
    [SerializeField] private int grade; //등급을 표기할 필요가 있을지 의문.
    public virtual int Grade {get{return grade;}}
    [SerializeField] private int speed;
    public virtual int Speed {get{return speed;}}
    [SerializeField] private int control;
    public virtual int Control {get{return control;}}
    [SerializeField] private int priceCrystal;
    public virtual int PriceCrystal {get{return priceCrystal;}}
    [SerializeField] private int pricePoint;
    public virtual int PricePoint {get{return pricePoint;}}
    [SerializeField] private int maxEntry;
    public virtual int MaxEntry {get{return maxEntry;}}
    public int entry;
    [SerializeField] private double chargeTime;
    public virtual double ChargeTime {get{return chargeTime;}}
    public double time;
    [HideInInspector] public bool isBought;
    public void tickTime(double time) {
        this.time += time;
        if(this.time >= ChargeTime) {
            entry++;
            this.time -= ChargeTime;
            if(entry == maxEntry) CharacterManager.Instance.chargeTime -= tickTime;
        }
    }
}

public class CharacterManager : Singleton<CharacterManager> {
	[SerializeField] private SportPlayer[] players;
    [HideInInspector] public int currentCharacter = 0;
    [HideInInspector] public float getSpeedPercent {get{return players[currentCharacter].Speed * 0.01f;}}
    [HideInInspector] public float getControlPercent {get{return players[currentCharacter].Control * 0.01f;}}
    private DateTime startTime;
    public delegate void entryTime(double time);
    public entryTime chargeTime;
    protected CharacterManager() { }
	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
    private void Start() {
        loadData();
        checkStartEntry();
        setCharacterTime();
        StartCoroutine(timeCheck());
    }

    private class PlayerData {
        public bool isBought;
        public double time;
        public int entry;
    }

    private void loadData() {
        if(string.IsNullOrEmpty(PlayerPrefs.GetString("characters"))) return;
        object lists = ANMiniJSON.Json.Deserialize(PlayerPrefs.GetString("characters"));
        IList collection = (IList)lists;
        for(int i = 0; i < players.Length; i++) {
            PlayerData data = new PlayerData();
            JsonUtility.FromJsonOverwrite((string)collection[i], data);
            players[i].isBought = data.isBought;
            players[i].entry = data.entry;
            players[i].time = data.time;
        }
        startTime = DateTime.Parse(PlayerPrefs.GetString("time", DateTime.UtcNow.ToString()));
    }

    private void saveData() {
        PlayerData[] datas = new PlayerData[players.Length];
        string[] dataString = new string[players.Length];
        for(int i = 0; i < players.Length; i++) {
            datas[i] = new PlayerData();
            datas[i].isBought = players[i].isBought;
            datas[i].entry = players[i].entry;
            datas[i].time = players[i].time;
            dataString[i] = JsonUtility.ToJson(datas[i]);
        }
        PlayerPrefs.SetString("characters", ANMiniJSON.Json.Serialize(dataString));
        PlayerPrefs.SetString("time", DateTime.UtcNow.ToString());
    }

    public string getName(int num) {
        return players[num].Name;
    }
    
    public int getSpeed(int num) {
        return players[num].Speed;
    }
    
    public int getControl(int num) {
        return players[num].Control;
    }

    public bool buyIt(int num) {
        return players[num].isBought;
    }

    public void sold(int num) {
        players[num].isBought = true;
        currentCharacter = num;
        saveData();
    }

    public int getPriceCrystal(int num) {
        return players[num].PriceCrystal;
    }
    
    public int getPricePoint(int num) {
        return players[num].PricePoint;
    }

    public int getMaxEntry(int num) {
        return players[num].MaxEntry;
    }

    public int getCurrentEntry(int num) {
        return players[num].entry;
    }

    public double getLeftTime(int num) {
        return players[num].ChargeTime - players[num].time;
    }

    public bool playGame() {
        if(players[currentCharacter].entry == 0) return false;
        if(players[currentCharacter].MaxEntry == players[currentCharacter].entry) {
            chargeTime += players[currentCharacter].tickTime;
        }
        players[currentCharacter].entry--;
        return true;
    }

    private void checkStartEntry() {
        DateTime currentTime = System.DateTime.UtcNow;
        TimeSpan passedTime = currentTime-startTime;
        double totalSeconds = passedTime.TotalSeconds;
        for(int i = 0; i < players.Length; i++) {
            if(players[i].MaxEntry == players[i].entry) continue;
            double charSecond = totalSeconds + players[i].time;
            int addEntry = (int)(charSecond / players[i].ChargeTime);
            if(players[i].entry + addEntry >= players[i].MaxEntry) { //최대 수치 넘을 경우
                players[i].entry = players[i].MaxEntry;
                players[i].time = 0.0;
            }
            else {
                players[i].entry += addEntry;
                players[i].time = charSecond % players[i].ChargeTime;
            }
        }
    }

    private IEnumerator timeCheck() {
        WaitForSeconds second = new WaitForSeconds(1f);
        DateTime currentTime;
        TimeSpan passedTime;
        while(true) {
            currentTime = System.DateTime.UtcNow;
            passedTime = currentTime-startTime;
            startTime = currentTime;
            if(chargeTime != null) chargeTime(passedTime.TotalSeconds);
            yield return second;
        }
    }

    private void setCharacterTime() {
        for(int i = 0; i < players.Length; i++)
            if(players[i].entry < players[i].MaxEntry)
                chargeTime += players[i].tickTime;
    }

    private void OnApplicationQuit() {
        Application.CancelQuit();
        StopCoroutine(timeCheck());
        saveData();
        Application.Quit();
    }

    private void OnDisable() {
        saveData();
    }
}
