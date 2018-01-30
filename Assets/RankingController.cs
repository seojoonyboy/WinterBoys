using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using System;
public class RankingController : MonoBehaviour {
    public GameObject Raw;
    public GameObject[]
        panels;
    NetworkManager networkManager;
    private void Start() {
        networkManager = NetworkManager.Instance;

        dataReq();
    }

    private void dataReq() {
        networkManager.getRanksByDist(ranksByDistCallback, SportType.DOWNHILL);
        networkManager.getRanksByDist(ranksByDistCallback, SportType.SKIJUMP);

        networkManager.getRanksByPoint(ranksByPointCallback, SportType.DOWNHILL);
        networkManager.getRanksByPoint(ranksByPointCallback, SportType.SKIJUMP);
    }

    private void ranksByDistCallback(HTTPResponse resp, SportType type) {
        Transform parent = null;
        switch (type) {
            case SportType.DOWNHILL:
                parent = panels[0].transform.Find("DistRaws").transform;
                break;
            case SportType.SKIJUMP:
                parent = panels[1].transform.Find("DistRaws").transform;
                break;
        }
        GameObject raw = Instantiate(Raw);

        Tmp tmp = new Tmp();
        JsonUtility.FromJsonOverwrite(resp.DataAsText, tmp);
        setInfo(raw);
    }

    private void ranksByPointCallback(HTTPResponse resp, SportType type) {
        Transform parent = null;
        switch (type) {
            case SportType.DOWNHILL:

                break;
            case SportType.SKIJUMP:

                break;
        }
    }

    private void setInfo(GameObject raw) {
        Text rankingNum = raw.transform.Find("Panel/RankingNum").GetComponent<Text>();
        Text nickName = raw.transform.Find("Panel/NickName").GetComponent<Text>();
        Text record = raw.transform.Find("Panel/Record").GetComponent<Text>();


    }

    private void OnEnable() {

    }

    public class Tmp {

    }
}
