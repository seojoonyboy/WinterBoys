using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public percentages dh_percentages;
    public percentages sj_percentages;
    public percentages st_percentages;

    public int pixelPerUnit = 1024;

    //유저 닉네임
    public string nickname = null;
    //유저 캐릭터 번호
    public int character = 0;

    public float[]
        poll_intervals,                     //폴 사이 정보
        vertical_intervals,                 //행간 정보
        bonus_times,                        //보너스 타임 관련 정보
        points;                             //포인드 획득 관련 정보

    public float[]
        skeleton_bonus_times,
        skeleton_dangers,
        skeleton_point,
        skeleton_stats;

    public float panelty_time;              //폴 통과하지 못한 경우 패널티
    public int startTime;
    public float[] highestScores;                    //각 종목별 최고 점수

    public enum tutorialEnum { SELECT, READY, SHOP, CHARACTER, DOWNHLL, SKELETON, SKIJUMP }
    private bool[] tutorialList;
    public tutorialEnum tutorialSports = tutorialEnum.SELECT;
    
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        //RemoteSettings.ForceUpdate();
        init();
    }
    public void init() {
        Application.targetFrameRate = 60;
        tutorialDataLoad();
        //startTime = RemoteSettings.GetInt("startTime");

        //getRemoteData("Downhill_bonus_times", ref bonus_times);
        //getRemoteData("Downhill_vertical_interval", ref vertical_intervals);
        //getRemoteData("Downhill_poll_interval", ref poll_intervals);
        //getRemoteData("Downhill_miss", ref panelty_time);
        //getRemoteData("Downhill_Point", ref points);

        //getRemoteData("Skeleton_bonus_times", ref skeleton_bonus_times);
        //getRemoteData("Skeleton_dangers", ref skeleton_dangers);
        //getRemoteData("Skeleton_point", ref skeleton_point);
        //getRemoteData("Skeleton_stats", ref skeleton_stats);
    }

    private void getRemoteData(string remote_key,ref float[] data) {
        string str = RemoteSettings.GetString(remote_key);
        string[] spl_str = str.Split(',');
        data = new float[spl_str.Length];
        for (int i = 0; i < spl_str.Length; i++) {
            data[i] = float.Parse(spl_str[i]);
        }
    }

    private void getRemoteData(string remote_key, ref float data) {
        string str = RemoteSettings.GetString(remote_key);
        data = float.Parse(str);
    }

    [System.Serializable]
    public class percentages {
        public float[] values;
    }

    private void tutorialDataLoad() {
        string data = PlayerPrefs.GetString("tutorial", "[false,false,false,false,false,false,false]");
        IList list = ANMiniJSON.Json.Deserialize(data) as IList;
        tutorialList = new bool[list.Count];
        for(int i = 0; i < list.Count; i++)
            tutorialList[i] = (bool)list[i];
    }

    public bool isTutorial(tutorialEnum tuto) {
        return tutorialList[(int)tuto];
    }

    public void tutorialDone(tutorialEnum tuto) {
        tutorialList[(int)tuto] = true;
        string data = ANMiniJSON.Json.Serialize(tutorialList);
        PlayerPrefs.SetString("tutorial", data);
    }
}