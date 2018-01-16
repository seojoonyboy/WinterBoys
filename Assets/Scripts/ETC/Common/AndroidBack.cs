using UnityEngine;

public class AndroidBack : MonoBehaviour {

	void OnEnable() {
		GameManager.Instance.setQuitModal(gameObject);
		Time.timeScale = 0;
	}

	void OnDisable() {
		GameManager.Instance.releaseQuitModal();
		Time.timeScale = 1;
		SoundManager.Instance.Play(SoundManager.SoundType.EFX, "gameSelBtn");
	}
}
