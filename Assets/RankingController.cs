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
    [SerializeField] private Sprite myPanel;
    NetworkManager networkManager;
    private void Awake() {
        networkManager = NetworkManager.Instance;
    }

    private void dataReq() {
        if(SaveManager.Instance.getRecord(SportType.DOWNHILL) != 0f) {
            networkManager.getRanksByDist(ranksByDistCallback, SportType.DOWNHILL);
            networkManager.getRanksByPoint(ranksByPointCallback, SportType.DOWNHILL);
            panels[0].transform.GetChild(3).gameObject.SetActive(false);
        } 
        else {
            panels[0].transform.GetChild(3).gameObject.SetActive(true);
        }
        if(SaveManager.Instance.getRecord(SportType.DOWNHILL) != 0f) {
            networkManager.getRanksByDist(ranksByDistCallback, SportType.SKIJUMP);
            networkManager.getRanksByPoint(ranksByPointCallback, SportType.SKIJUMP);
            panels[1].transform.GetChild(3).gameObject.SetActive(false);
        }
        else {
            panels[1].transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    private void ranksByDistCallback(HTTPResponse resp, SportType type) {
        if (!resp.IsSuccess) {
            if(resp.StatusCode == 404) {
                Debug.LogError("네트워크 에러 발생");
                int num;
                if(type == SportType.DOWNHILL) num = 0;
                else num = 1;
                GameObject error = panels[num].transform.GetChild(3).gameObject;
                error.SetActive(true);
                error.GetComponent<I2.Loc.Localize>().mTerm = "rank_error";
            }
            return;
        }
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
        setInfo(parent, dataSet, true);
    }

    private void ranksByPointCallback(HTTPResponse resp, SportType type) {
        if (!resp.IsSuccess) {
            if (resp.StatusCode == 404) {
                Debug.LogError("네트워크 에러 발생");
            }
            return;
        }
        Transform parent = null;
        switch (type) {
            case SportType.DOWNHILL:
                parent = panels[0].transform.Find("PointRaws").transform;
                break;
            case SportType.SKIJUMP:
                parent = panels[1].transform.Find("PointRaws").transform;
                break;
        }

        DataSet[] dataSet = JsonHelper.getJsonArray<DataSet>(resp.DataAsText);
        setInfo(parent, dataSet, false);
    }

    private void setInfo(Transform parent, DataSet[] dataSet, bool isDistance) {
        foreach(DataSet data in dataSet) {
            GameObject raw = Instantiate(Raw);
            raw.transform.SetParent(parent, false);

            raw.transform.localScale = Vector3.one;
            raw.transform.localPosition = Vector3.zero;

            Image panel = raw.GetComponentInChildren<Image>();
            Text rankingNum = raw.transform.Find("Panel/RankingNum").GetComponent<Text>();
            Text nickName = raw.transform.Find("Panel/NickName").GetComponent<Text>();
            Text record = raw.transform.Find("Panel/Record").GetComponent<Text>();

            if(data.user.device_id == SystemInfo.deviceUniqueIdentifier) panel.sprite = myPanel;
            //if(data.user.device_id == "b5f543b0eb99661e381d1f18e2c74d7fa9bc0c619a38be706e") panel.sprite = myPanel;
            if(I2.Loc.LocalizationManager.CurrentLanguage.CompareTo("English") == 0) {
                switch(data.rank) {
                    case 1:
                    rankingNum.text = data.rank + "st";
                    break;
                    case 2:
                    rankingNum.text = data.rank + "nd";
                    break;
                    case 3:
                    rankingNum.text = data.rank + "rd";
                    break;
                    default:
                    rankingNum.text = data.rank + "th";
                    break;
                }
            }
            else
                rankingNum.text = data.rank + "위";
            nickName.text = data.user.nickname;
            if (isDistance) {
                record.text = data.distance + "M";
            }
            else {
                record.text = data.point.ToString();
            }
        }
    }

    private void OnEnable() {
        panelInit();
        destroy();
        dataReq();
    }

    private void destroy() {
        foreach (GameObject panel in panels) {
            foreach (Transform raw in panel.transform.Find("PointRaws")) {
                Destroy(raw.gameObject);
            }
            foreach (Transform raw in panel.transform.Find("DistRaws")) {
                Destroy(raw.gameObject);
            }
        }
    }

    private void panelInit() {
        foreach (GameObject panel in panels) {
            panel.transform.Find("ToggleGroup/DistRankingToggle").GetComponent<Toggle>().isOn = true;
        }
        transform.Find("ButtonArea/Downhill_Button").GetComponent<Toggle>().isOn = true;
    }

    [System.Serializable]
    public class DataSet {
        public int id;
        public User user;
        public int rank;
        public int distance;
        public int point;
    }

    [System.Serializable]
    public class User {
        public int id;
        public string device_id;
        public string nickname;
    }
}
