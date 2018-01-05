using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour {
    GameManager gm;
    CharacterManager cm;
    [SerializeField] private CharStat charStat;
    [SerializeField] private Sprite[] charSprites;
    [SerializeField] private Button nextBtn;
    private int charIndex = -1;

    private void Awake() {
        gm = GameManager.Instance;
        cm = CharacterManager.Instance;
    }

    private void Start() {
        nextBtn.onClick.AddListener(nextCharacter);
        nextCharacter();
    }

    private void nextCharacter() {
        charIndex++;
        if(charIndex >= 3) charIndex = 0;
        gm.character = charIndex;
        string charNameString = string.Format("<color=yellow>\"{0}\"</color> 선수로\n등록하실래요?", cm.getName(charIndex));
        charStat.setData(charSprites[charIndex], charNameString, cm.getSpeed(charIndex), cm.getControl(charIndex));
        SoundManager.Instance.Play(SoundManager.SoundType.EFX, "charChangeBtn");
    }
}
