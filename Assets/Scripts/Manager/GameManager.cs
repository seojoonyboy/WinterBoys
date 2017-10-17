using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public Sprite[] players;

    public int pixelPerUnit = 512;

    //유저 닉네임
    public string nickname = null;
    //유저 캐릭터 번호
    public int character = 0;

    public float
        total_limit_time = 0.0f,            //첫 시작시 남은 시간
        inc_time_per_num = 20,              //제한시간 변동 기준인 폴의 갯수
        inc_time_per_amount = 15,           //폴 통과에 따른 제한시간 증가량
        dec_time_per_missed = 5,            //폴을 통과하지 못한 경우 제한시간 감소량
        poll_interval_default = 500,        //폴 사이 간격 기본값
        poll_interval_dec_per_num = 25,     //폴 간격 변동 기준인 폴의 갯수
        poll_interval_dec_amount = 8.0f,    //폴 간격 감소량
        poll_interval_max_dec_count = 10,   //폴 간격 감소 최대 횟수
        row_interval_default = 300,         //행간 간격 기본값
        row_interval_inc_per_num = 20,      //행간 간격 감소 기준인 폴의 갯수
        row_interval_inc_per_amount = 25.0f,//행간 간격 증가량
        row_interval_max_inc_count = 10,    //행간 간격 감소 최대 횟수
        row_total_default_min_move_amount = 0,      //행 전체 기본 최소 이동량
        row_total_default_max_move_amount = 30,     //행 전체 기본 최대 이동량
        row_total_move_per_num = 15,        //행 전체 이동 기준인 폴의 갯수
        row_total_move_amount = 15,         //행 전체 이동량
        row_total_min_move_amount = 15,     //행 전체 변동 최소 이동량
        row_total_max_move_amount = 25,     //행 전체 변동 최대 이동량
        combo_per_num = 1,                  //콤보 기준 폴의 갯수
        miss_penalty = 5.0f;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        init();
    }

    private void init() {
        string str;
        str = RemoteSettings.GetString("Downhill_time");
        string[] spl_str = str.Split(',');

        inc_time_per_num = float.Parse(spl_str[0]);
        inc_time_per_amount = float.Parse(spl_str[1]);

        str = RemoteSettings.GetString("Downhill_poll_row_interval");
        spl_str = str.Split(',');
        row_interval_default = float.Parse(spl_str[0]);
        row_interval_inc_per_num = float.Parse(spl_str[1]);
        row_interval_inc_per_amount = float.Parse(spl_str[2]);
        row_interval_max_inc_count = float.Parse(spl_str[3]);

        str = RemoteSettings.GetString("Downhill_row_total_move");

        spl_str = str.Split(',');
        row_total_default_min_move_amount = float.Parse(spl_str[0]);
        row_total_default_max_move_amount = float.Parse(spl_str[1]);
        row_total_move_per_num = float.Parse(spl_str[2]);
        row_total_move_amount = float.Parse(spl_str[3]);
        row_total_min_move_amount = float.Parse(spl_str[4]);
        row_total_max_move_amount = float.Parse(spl_str[5]);

        str = RemoteSettings.GetString("Downhill_poll_interval");

        spl_str = str.Split(',');
        poll_interval_default = float.Parse(spl_str[0]);
        poll_interval_dec_per_num = float.Parse(spl_str[1]);
        poll_interval_dec_amount = float.Parse(spl_str[2]);
        poll_interval_max_dec_count = float.Parse(spl_str[3]);

        str = RemoteSettings.GetString("Downhill_miss");
        miss_penalty = float.Parse(str);
    }

    public void gameOver() {
        Time.timeScale = 0;
    }
}