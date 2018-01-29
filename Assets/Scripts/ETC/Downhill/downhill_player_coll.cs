using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class downhill_player_coll : MonoBehaviour {
    public Ski_PlayerController controller;
    public DownhillManager dM;
    public BoardManager bM;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Tile") {
            Vector2 pos = collision.transform.position;
            if (pos.y <= bM.lastTilePos.y + 14.2f) {
                bM.addToBoard();
            }
        }

        if (collision.tag == "DH_leftTile") {
            //dM.OnGameOver(DownhillManager.GameoverReason.SIDETILE);
            Vector3 contactPoint = new Vector3(3, 0, 0);
            controller.bounce(contactPoint);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_bigCrash");
        }

        if(collision.tag == "DH_rightTile") {
            Vector3 contactPoint = new Vector3(-3, 0, 0);
            controller.bounce(contactPoint);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_bigCrash");
        }

        if (collision.tag == "Item") {
            GameObject obj = collision.gameObject;
            controller.itemCheck(obj);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "DH_leftTile") {
            //dM.OnGameOver(DownhillManager.GameoverReason.SIDETILE);
            Vector3 contactPoint = new Vector3(3, 0, 0);
            controller.bounce(contactPoint);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_bigCrash");
        }

        if (collision.tag == "DH_rightTile") {
            Vector3 contactPoint = new Vector3(-3, 0, 0);
            controller.bounce(contactPoint);

            SoundManager.Instance.Play(SoundManager.SoundType.EFX, "dh_bigCrash");
        }
    }
}
