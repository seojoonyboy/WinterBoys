using System.Collections;
using System.Collections.Generic;
using System;
using BestHTTP;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

    //private string url = "http://ec2-13-124-187-31.ap-northeast-2.compute.amazonaws.com";
    private string url = "http://13.125.17.68";
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
        string str = null;
        switch (type) {
            case SportType.DOWNHILL:

                break;
            case SportType.SKIJUMP:

                break;
        }

        if(str == null) { return; }

        HTTPRequest request = new HTTPRequest(new Uri(url + str), HTTPMethods.Get);
        //request.AddHeader("Authorization", "Token " + token.key);
        StartCoroutine(AccessNetwork(request, callback, type));
    }

    /// <summary>
    /// 포인트 기준 순위 가져오기
    /// </summary>
    public void getRanksByPoint(networkResult callback, SportType type) {
        string str = null;
        switch (type) {
            case SportType.DOWNHILL:

                break;
            case SportType.SKIJUMP:

                break;
        }

        if (str == null) { return; }

        HTTPRequest request = new HTTPRequest(new Uri(url + str), HTTPMethods.Get);
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