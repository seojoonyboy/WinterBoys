using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {
    GameManager gm;
    CharacterManager cm;
    [SerializeField] private CharStat charStat;
    [SerializeField] private Sprite[] charSprites;
    [SerializeField] private Button beforeBtn;
    [SerializeField] private Button nextBtn;
    private int charIndex = -1;

    private void Awake() {
        cm = CharacterManager.Instance;
    }

    private void Start() {
        beforeBtn.onClick.AddListener(() => nextCharacter(false));
        nextBtn.onClick.AddListener(() => nextCharacter(true));
        nextCharacter(true);
    }

    private void nextCharacter(bool next) {
        if(next) charIndex++;
        else charIndex--;
        if(charIndex >= 3) charIndex = 0;
        if(charIndex < 0) charIndex = 2;
        cm.currentCharacter = charIndex;
        charStat.setData(charSprites[charIndex], cm.getName(charIndex), cm.getSpeed(charIndex), cm.getControl(charIndex));
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "charChangeBtn");
    }
}
