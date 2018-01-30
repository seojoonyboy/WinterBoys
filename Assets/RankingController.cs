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
        DataSet[] dataSet = JsonHelper.getJsonArray<DataSet>(resp.DataAsText);
        setInfo(parent, dataSet);
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

    private void setInfo(Transform parent, DataSet[] dataSet) {
        foreach(DataSet data in dataSet) {
            GameObject raw = Instantiate(Raw);
            raw.transform.SetParent(parent, false);

            raw.transform.localScale = Vector3.one;
            raw.transform.localPosition = Vector3.zero;

            Text rankingNum = raw.transform.Find("Panel/RankingNum").GetComponent<Text>();
            Text nickName = raw.transform.Find("Panel/NickName").GetComponent<Text>();
            Text record = raw.transform.Find("Panel/Record").GetComponent<Text>();


        }
    }

    private void OnEnable() {

    }

    [System.Serializable]
    public class DataSet {
        public string nickName;
        public int rankingNum;
    }
}
