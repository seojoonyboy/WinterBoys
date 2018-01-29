using System.Collections;
using System.Collections.Generic;
using System;
using BestHTTP;
using UnityEngine;

class Token {
    public string key;
    public string date;
    public int user;

    public Token() {
        key = null;
        date = null;
        user = 0;
    }
}

public class NetworkManager : Singleton<NetworkManager> {
    protected NetworkManager() { }

    //private string url = "http://ec2-13-124-187-31.ap-northeast-2.compute.amazonaws.com";
    private string url = "http://13.125.17.68";
    //public GameObject loadingImage;

    public delegate void networkResult(HTTPResponse response);
    //private networkResult callback;
    private Token token;

    public void login(networkResult callback) {
        HTTPRequest request = new HTTPRequest(new Uri(url + "/signin"), HTTPMethods.Post);
        request.AddField("device_id", SystemInfo.deviceUniqueIdentifier);
        //callback += getToken;
        StartCoroutine(AccessNetwork(request, callback));
    }

    public void signup(string nickname, networkResult callback) {
        HTTPRequest request = new HTTPRequest(new Uri(url + "/signup"), HTTPMethods.Post);
        request.AddField("device_id", SystemInfo.deviceUniqueIdentifier);
        request.AddField("nick_name", nickname);
        StartCoroutine(AccessNetwork(request, callback));
    }

    public void getInfo(networkResult callback) {
        if (token == null) {
            //Debug.LogWarning("need Token");
            return;
        }
        HTTPRequest request = new HTTPRequest(new Uri(url + "/me"), HTTPMethods.Get);
        request.AddHeader("Authorization", "Token " + token.key);
        StartCoroutine(AccessNetwork(request, callback));
    }

    public IEnumerator AccessNetwork(HTTPRequest request, networkResult callback) {
        //loadingImage.SetActive(true);
        request.Send();
        yield return request;
        if (callback != null) callback(request.Response);
        //Debug.Log(request.Uri.ToString());
        //Debug.Log(request.Response.DataAsText);
        //loadingImage.SetActive(false);
    }

    public void getToken(HTTPResponse response) {
        bool isSuccess = response != null && response.IsSuccess;
        if (!isSuccess) return;
        token = new Token();
        JsonUtility.FromJsonOverwrite(response.DataAsText, token);
    }

    /// <summary>
    /// 인게임 종료 후 점수 서버 전달
    /// callback으로 랭킹정보 받기?
    /// </summary>
    public void sendMyRank(networkResult callback) {

    }

    /// <summary>
    /// 전체 랭킹 보기
    /// </summary>
    /// <param name="callback"></param>
    public void GetTotalRank(networkResult callback) {

    }
}