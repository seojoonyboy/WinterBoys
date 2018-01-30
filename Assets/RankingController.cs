using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RankingController : MonoBehaviour {
    private PanelType activeType;
    public GameObject[] 
        panels,
        active_headers,
        deactive_headers;

    private void OnEnable() {
        activeType = PanelType.Downhill;
        TogglePanel("Downhill");
    }

    public void TogglePanel(string str) {
        //DB에서 정보 불러오기
        activeType = (PanelType)Enum.Parse(typeof(PanelType), str);
        int index = (int)activeType;

        initPanel(index);
    }

    private void initPanel(int index) {
        GameObject panel = null;
        for(int i=0; i<panels.Length; i++) {
            if(index == i) {
                panel = panels[i];
            }
        }
        panel.SetActive(true);

        panel.transform.Find("DistRaws").gameObject.SetActive(true);
        panel.transform.Find("PointRaws").gameObject.SetActive(false);
    }

    public enum PanelType {
        Downhill,
        Skijump
    }
}
