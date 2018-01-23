using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour {
	private GameManager gm;
	private SoundManager sm;
	
	[SerializeField] Toggle bgmToggle;
	[SerializeField] Toggle efxToggle;
	[SerializeField] Toggle vibrateToggle;
	[SerializeField] Sprite on;
	[SerializeField] Sprite off;

	private void Awake() {
		gm = GameManager.Instance;
		sm = SoundManager.Instance;
		setBtn();
	}

	private void setBtn() {
		bgmToggle.onValueChanged.AddListener(setBgmToggle);
		efxToggle.onValueChanged.AddListener(setEfxToggle);
		vibrateToggle.onValueChanged.AddListener(setVibrateToggle);
	}


	private void Start() {
		init();
	}

	private void init() {
		bgmToggle.isOn = gm.optionData.bgm;
		efxToggle.isOn = gm.optionData.efx;
		vibrateToggle.isOn = gm.optionData.vibrate;
		bgmToggle.image.sprite = gm.optionData.bgm ? on : off;
		efxToggle.image.sprite = gm.optionData.efx ? on : off;
		vibrateToggle.image.sprite = gm.optionData.vibrate ? on : off;
	}

	private void setBgmToggle(bool value) {
		bgmToggle.image.sprite = value ? on : off;
		gm.optionData.bgm = value;
		gm.optionSave();
		sm.Play(SoundManager.SoundType.EFX, "gameSelBtn");
		sm.setOption();
	}

	private void setEfxToggle(bool value) {
		efxToggle.image.sprite = value ? on : off;
		gm.optionData.efx = value;
		gm.optionSave();
		sm.Play(SoundManager.SoundType.EFX, "gameSelBtn");
		sm.setOption();
	}

	private void setVibrateToggle(bool value) {
		vibrateToggle.image.sprite = value ? on : off;
		gm.optionData.vibrate = value;
		gm.optionSave();
		sm.Play(SoundManager.SoundType.EFX, "gameSelBtn");
	}
}
