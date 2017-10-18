using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {
    public GameManager gm;
    public string[] charNames;

    private Text charName;
    private int charIndex = 0;

    private void Awake() {
        gm = GameManager.Instance;

        string str = RemoteSettings.GetString("Characters_name");
        Debug.Log("str : " + str);
        string[] spl_str = str.Split(',');
        for (int i=0; i<charNames.Length; i++) {
            charNames[i] = spl_str[i];
        }

        charName = transform.Find("Text").GetComponent<Text>();

        gm.nickname = charName.text;
        gm.character = charIndex;
    }

    private void OnEnable() {
        charIndex = 0;
        charName.text = charNames[charIndex];
    }

    public void arrowClicked(int dir) {
        switch (dir) {
            //right
            case 1:
                if(charIndex == charNames.Length - 1) {
                    charIndex = 0;
                }
                else {
                    charIndex++;
                }
                break;
            //left
            case -1:
                if(charIndex == 0) {
                    charIndex = charNames.Length - 1;
                }
                else {
                    charIndex--;
                }
                break;
        }
        charName.text = charNames[charIndex];
        gm.character = charIndex;
    }
}
