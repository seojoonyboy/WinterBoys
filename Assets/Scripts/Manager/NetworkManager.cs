using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
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
    /// 전체 랭킹 보기
    /// </summary>
    /// <param name="callback"></param>
    public void GetTotalRank(networkResult callback) {

    }
}