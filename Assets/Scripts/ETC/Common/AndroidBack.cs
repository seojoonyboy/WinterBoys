using UnityEngine;

public class AndroidBack : MonoBehaviour {

	void OnEnable() {
		GameManager.Instance.setQuitModal(gameObject);
	}

	void OnDisable() {
			GameManager.Instance.releaseQuitModal();
			//AudioManager.Instance.playSound("start_button");
	}
}
