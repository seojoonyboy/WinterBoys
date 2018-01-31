using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using Firebase.Analytics;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

    //private string url = "http://ec2-13-124-187-31.ap-northeast-2.compute.amazonaws.com";
    private string url = "https://apptest.fbl.kr/winterboy";
    //public GameObject loadingImage;

    public delegate void networkResult(HTTPResponse response, SportType type);

    public IEnumerator AccessNetwork(HTTPRequest request, networkResult callback, SportType type) {
        //loadingImage.SetActive(true);
        request.Send();
        yield return request;
        if (callback != null) callback(request.Response, type);
        //Debug.Log(request.Uri.ToString());
        //Debug.Log(request.Response.DataAsText);
        //loadingImage.SetActive(false);
    }

    public IEnumerator AccessNetwork(HTTPRequest request, networkResult callback, SportType type, Field[] fields) {
        foreach(Field field in fields) {
            request.AddField(field.key, field.value);
        }
        request.Send();
        yield return request;
        if (callback != null) callback(request.Response, type);
        //에러 날 경우
        if(!request.Response.IsSuccess) FirebaseAnalytics.LogEvent("InternetError","Code", request.Response.StatusCode);
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 거리 기준 순위 가져오기
    /// </summary>
    public void getRanksByDist(networkResult callback, SportType type) {
        StringBuilder builderStr = new StringBuilder();
        builderStr
            .Append(url)
            .Append("/rank_by_id/");

        switch (type) {
            case SportType.DOWNHILL:
                builderStr.Append("downhill/");
                break;
            case SportType.SKIJUMP:
                builderStr.Append("skijump/");
                break;
        }
        builderStr
            .Append("distance/")
            //.Append(SystemInfo.deviceUniqueIdentifier)
            .Append("b5f543b0eb99661e381d1f18e2c74d7fa9bc0c619a38be706e")
            .Append("/");
        HTTPRequest request = new HTTPRequest(new Uri(builderStr.ToString()), HTTPMethods.Get);
        //request.AddHeader("Authorization", "Token " + token.key);
        StartCoroutine(AccessNetwork(request, callback, type));
    }

    /// <summary>
    /// 포인트 기준 순위 가져오기
    /// </summary>
    public void getRanksByPoint(networkResult callback, SportType type) {
        StringBuilder builderStr = new StringBuilder();
        builderStr
            .Append(url)
            .Append("/rank_by_id/");

        switch (type) {
            case SportType.DOWNHILL:
                builderStr.Append("downhill/");
                break;
            case SportType.SKIJUMP:
                builderStr.Append("skijump/");
                break;
        }
        builderStr
            .Append("point/")
            .Append("b5f543b0eb99661e381d1f18e2c74d7fa9bc0c619a38be706e")
            .Append("/");
        HTTPRequest request = new HTTPRequest(new Uri(builderStr.ToString()), HTTPMethods.Get);
        //request.AddHeader("Authorization", "Token " + token.key);
        StartCoroutine(AccessNetwork(request, callback, type));
    }

    /// <summary>
    /// 인게임 종료시 내 기록 db 전달
    /// callback으로 종료 모달 내 랭킹 정보 구성
    /// </summary>
    public void postRecord(networkResult callback, SportType type, int dist, int point) {
        StringBuilder builderStr = new StringBuilder();
        builderStr
            .Append(url)
            .Append("/record_rank/");

        Field[] fields = new Field[6];
        for(int i=0; i<fields.Length; i++) {
            fields[i] = new Field();
        }
        fields[0].key = "key";
        fields[0].value = "gudckddhfflavlr";

        fields[1].key = "device_id";
        //fields[1].value = SystemInfo.deviceUniqueIdentifier;
        fields[1].value = "b5f543b0eb99661e381d1f18e2c74d7fa9bc0c619a38be706e";

        fields[2].key = "nickname";
        fields[2].value = "nickname";

        fields[3].key = "distance";
        fields[3].value = dist.ToString();

        fields[4].key = "point";
        fields[4].value = point.ToString();

        fields[5].key = "event_name";
        switch (type) {
            case SportType.DOWNHILL:
                fields[5].value = "downhill";
                break;
            case SportType.SKIJUMP:
                fields[5].value = "skijump";
                break;
        }
        HTTPRequest request = new HTTPRequest(new Uri(builderStr.ToString()), HTTPMethods.Post);
        StartCoroutine(AccessNetwork(request, callback, type, fields));
    }

    /// <summary>
    /// 전체 랭킹 보기
    /// </summary>
    /// <param name="callback"></param>
    public void GetTotalRank(networkResult callback) {

    }

    public class Field {
        public string key;
        public string value;
    }
}