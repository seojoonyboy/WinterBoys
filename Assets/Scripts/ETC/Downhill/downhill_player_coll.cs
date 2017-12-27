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
            if (pos.y <= bM.lastTilePos.y + 2) {
                bM.addToBoard();
            }
        }

        if (collision.tag == "TileEnd") {
            dM.OnGameOver(DownhillManager.GameoverReason.SIDETILE);
        }

        if (collision.tag == "Item") {
            GameObject obj = collision.gameObject;
            controller.itemCheck(obj);
        }
    }
}
