using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShopController : MonoBehaviour {
	private SaveManager sm;
	private CharacterManager cm;
	private SoundManager sound;
	[SerializeField] private CharStat charStat;
	[SerializeField] private ResourceController topPanel;
	[SerializeField] private Button leftBtn;
	[SerializeField] private Button rightBtn;
	[SerializeField] private Sprite[] characterSprite;
	[SerializeField] private Button buyCrystalBtn;
	[SerializeField] private Button buyPointBtn;
	[SerializeField] private Button currentBtn;
	[SerializeField] private Button closeBtn;
	[SerializeField] private GameObject modal;
	[SerializeField] private GameObject tutorialModal;
	private int charNum;


	private void Awake() {
		sm = SaveManager.Instance;
		cm = CharacterManager.Instance;
		sound = SoundManager.Instance;
	}

	private void Start() {
		charNum = cm.currentCharacter;
		setButton();
		if(GameManager.Instance.isTutorial(GameManager.tutorialEnum.CHARACTER)) return;
        tutorialModal.SetActive(true);
        GameManager.Instance.tutorialDone(GameManager.tutorialEnum.CHARACTER);
	}

	private void OnEnable() {
		setCharData();
	}

	private void FixedUpdate() {
		charStat.setTime(cm.getMaxEntry(charNum), cm.getCurrentEntry(charNum), cm.getLeftTime(charNum));
	}

	private void setButton() {
		leftBtn.onClick.AddListener(() => {--charNum; setCharData();});
		rightBtn.onClick.AddListener(() => {++charNum; setCharData();});
		buyCrystalBtn.onClick.AddListener(buyCrystal);
		buyPointBtn.onClick.AddListener(buyPoint);
		closeBtn.onClick.AddListener(closeSelect);
		currentBtn.onClick.AddListener(charSelected);
	}

	private void setCharData() {
		if(charNum >= 3) charNum=0;
		if(charNum < 0) charNum=2;
		setText();
		setBuy();
	}

	private void setText() {
		charStat.setData(characterSprite[charNum], cm.getName(charNum), cm.getSpeed(charNum), cm.getControl(charNum));
	}

	private void setBuy() {
		bool isSold = cm.buyIt(charNum);
		currentBtn.gameObject.SetActive(isSold);
		buyCrystalBtn.gameObject.SetActive(!isSold);
		buyPointBtn.gameObject.SetActive(!isSold);
		if(charNum == cm.currentCharacter)
			currentBtn.GetComponentInChildren<Text>().text = "출전중...";
		else
			currentBtn.GetComponentInChildren<Text>().text = "출전";
		buyCrystalBtn.GetComponentInChildren<Text>().text = string.Format("{0} 보석", cm.getPriceCrystal(charNum));
		buyPointBtn.GetComponentInChildren<Text>().text = string.Format("{0} 포인트", cm.getPricePoint(charNum));
	}

	private void buyCrystal() {
		bool canBuy = sm.getCrystalLeft() >= cm.getPriceCrystal(charNum);
		setModal(canBuy, true);
	}

	private void buyPoint() {
		bool canBuy = sm.getPointLeft() >= cm.getPricePoint(charNum);
		setModal(canBuy, false);
	}

	private void setModal(bool canBuy, bool isCrystal) {
		Button btn = modal.transform.Find("Panel/Buttons/YES").GetComponent<Button>();
		Text closeBtnText = modal.transform.Find("Panel/Buttons/NO/Text").GetComponent<Text>();
		Text text = modal.GetComponentInChildren<Text>();
		btn.onClick.RemoveAllListeners();
		btn.gameObject.SetActive(canBuy);
		text.text = canBuy ? "구매 하시겠습니까?" : "잔액이 부족합니다.";
		closeBtnText.text = canBuy ? "아니오" : "확인";
		modal.SetActive(true);
		if(!canBuy) return;
		btn.onClick.AddListener(() => {
			if(isCrystal) {
				if(sm.useCrystal(cm.getPriceCrystal(charNum)))
					sold();
			} else {
				if(sm.usePoint(cm.getPricePoint(charNum)))
					sold();
			}
		});
	}

	private void sold() {
		cm.sold(charNum);
		modal.SetActive(false);
		setCharData();
	}

	private void charSelected() {
		cm.currentCharacter = charNum;
		closeSelect();
	}

	private void closeSelect() {
		gameObject.SetActive(false);
		transform.parent.GetChild(1).SendMessage("setCharData", SendMessageOptions.DontRequireReceiver);
		Button btn = modal.transform.Find("Panel/Buttons/YES").GetComponent<Button>();
		Text closeBtnText = modal.transform.Find("Panel/Buttons/NO/Text").GetComponent<Text>();
		btn.gameObject.SetActive(true);
		btn.onClick.RemoveAllListeners();
		modal.GetComponentInChildren<Text>().text = "구매 하시겠습니까?";
		closeBtnText.text = "아니오";
		//sound.Play(SoundManager.SoundType.EFX, "closeBtn");
	}
	
}
