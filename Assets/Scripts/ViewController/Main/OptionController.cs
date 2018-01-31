using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour {
	private GameManager gm;
	private SoundManager sm;
	
	[SerializeField] Toggle bgmToggle;
	[SerializeField] Toggle efxToggle;
	[SerializeField] Toggle vibrateToggle;
    [SerializeField] Toggle languageToggle;
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
		languageToggle.onValueChanged.AddListener(setLanguageToggle);
	}


	private void Start() {
		init();
	}

	private void init() {
		string lang = I2.Loc.LocalizationManager.CurrentLanguage;
		bgmToggle.isOn = gm.optionData.bgm;
		efxToggle.isOn = gm.optionData.efx;
		vibrateToggle.isOn = gm.optionData.vibrate;
		//Debug.Log(lang);
		languageToggle.isOn = lang.CompareTo("English")!=0;
		bgmToggle.image.sprite = gm.optionData.bgm ? on : off;
		efxToggle.image.sprite = gm.optionData.efx ? on : off;
		vibrateToggle.image.sprite = gm.optionData.vibrate ? on : off;
		languageToggle.image.sprite = lang.CompareTo("English")!=0 ? on : off;
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

	private void setLanguageToggle(bool value) {
		I2.Loc.SetLanguage lang = GetComponentInChildren<I2.Loc.SetLanguage>();
		languageToggle.image.sprite = value ? on : off;
		lang._Language = value ? "Korean" : "English" ;
		lang.ApplyLanguage();
		sm.Play(SoundManager.SoundType.EFX, "gameSelBtn");
	}
}
