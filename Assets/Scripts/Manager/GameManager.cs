using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public Sprite[] players;

    public int pixelPerUnit = 1024;

    //유저 닉네임
    public string nickname = null;
    //유저 캐릭터 번호
    public int character = 0;

    public float[]
        poll_intervals,                     //폴 사이 정보
        vertical_intervals,                 //행간 정보
        pararell_intervals,                 //평행이동 관련 정보
        bonus_times,                        //보너스 타임 관련 정보
        points;                             //포인드 획득 관련 정보

    public float panelty_time;              //폴 통과하지 못한 경우 패널티
    public int startTime;
    public float[] highestScores;                    //각 종목별 최고 점수
    
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        RemoteSettings.ForceUpdate();
        init();
    }

    public void init() {
        string str;
        str = RemoteSettings.GetString("Downhill_bonus_times");
        string[] spl_str = str.Split(',');
        bonus_times = new float[spl_str.Length];
        for (int i = 0; i < spl_str.Length; i++) {
            bonus_times[i] = float.Parse(spl_str[i]);
        }

        str = RemoteSettings.GetString("Downhill_vertical_interval");
        spl_str = str.Split(',');
        vertical_intervals = new float[spl_str.Length];
        for (int i = 0; i < spl_str.Length; i++) {
            vertical_intervals[i] = float.Parse(spl_str[i]);
        }

        str = RemoteSettings.GetString("Downhill_pararell_intervals");
        spl_str = str.Split(',');
        pararell_intervals = new float[spl_str.Length];
        for (int i = 0; i < spl_str.Length; i++) {
            pararell_intervals[i] = float.Parse(spl_str[i]);
        }

        str = RemoteSettings.GetString("Downhill_poll_interval");

        spl_str = str.Split(',');
        poll_intervals = new float[spl_str.Length];
        for(int i=0; i<spl_str.Length; i++) {
            poll_intervals[i] = float.Parse(spl_str[i]);
        }

        str = RemoteSettings.GetString("Downhill_miss");
        panelty_time = float.Parse(str);

        startTime = RemoteSettings.GetInt("startTime");

        str = RemoteSettings.GetString("Downhill_Point");

        spl_str = str.Split(',');
        for (int i=0; i< spl_str.Length; i++) {
            points[i] = float.Parse(spl_str[i]);
        }
    }
}