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
    [SerializeField] private int chargeTime;
    public virtual float ChargeTime {get{return chargeTime;}}
    public float currentTime;
    [HideInInspector] public bool isBought;
}

public class CharacterManager : Singleton<CharacterManager> {
	[SerializeField] private SportPlayer[] players;
    [HideInInspector] public int currentCharacter = 0;
    [HideInInspector] public float getSpeedPercent {get{return players[currentCharacter].Speed * 0.01f;}}
    [HideInInspector] public float getControlPercent {get{return players[currentCharacter].Control * 0.01f;}}

    protected CharacterManager() { }
	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
    private void Start() {
        loadData();
    }

    private void loadData() {
        if(string.IsNullOrEmpty(PlayerPrefs.GetString("characters"))) return;
        object lists = ANMiniJSON.Json.Deserialize(PlayerPrefs.GetString("characters"));
        IList collection = (IList)lists;
        for(int i = 0; i < players.Length; i++)
            players[i].isBought = (bool)collection[i];
    }

    private void saveData() {
        bool[] datas = new bool[players.Length];
        for(int i = 0; i < players.Length; i++)
            datas[i] = players[i].isBought;
        //시간도 따로 추가할 필요는 있음...
        PlayerPrefs.SetString("characters",ANMiniJSON.Json.Serialize(datas));
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
}
