using UnityEngine;

public class AndroidBack : MonoBehaviour {

	void OnEnable() {
		GameManager.Instance.setQuitModal(gameObject);
	}

	void OnDisable() {
		GameManager.Instance.releaseQuitModal();
		SoundManager.Instance.Play(SoundManager.SoundType.EFX, "gameSelBtn");
	}
}
