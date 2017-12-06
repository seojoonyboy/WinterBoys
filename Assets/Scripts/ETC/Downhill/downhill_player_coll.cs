using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class downhill_player_coll : MonoBehaviour {
    private DownhillManager dM;
    private BoardManager bM;
    

    private void Awake() {
        dM = GameObject.Find("Manager").GetComponent<DownhillManager>();
        bM = GameObject.Find("BoardHolder").GetComponent<BoardManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Flag") {
            var flagComp = collision.GetComponent<FlagController>();
            var anim = collision.GetComponent<SkeletonAnimation>();
            anim.loop = false;

            if (flagComp.rayDir == FlagController.type.LEFT) {
                anim.AnimationName = "broken_left";
            }
            else if (flagComp.rayDir == FlagController.type.RIGHT) {
                anim.AnimationName = "broken_right";
            }

            bM.centers.RemoveAt(0);

            Rigidbody2D rb = dM.playerController.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x * 0.8f, rb.velocity.y * 0.8f);
        }

        if(collision.tag == "Tile") {
            Vector2 pos = collision.transform.position;
            if (pos.y <= bM.firstTilePos.y - 2) {
                if (!bM.isMade) {
                    bM.addToBoard();
                }
            }
        }

        if(collision.tag == "TileEnd") {
            dM.OnGameOver();
        }

        if(collision.tag == "Item") {
            GameObject obj = collision.gameObject;
            itemCheck(obj);
        }
    }

    public void itemCheck(GameObject obj) {
        if (obj.transform.tag == "Item") {
            dM.playerController.itemCheck(obj);
        }
        Destroy(obj);
    }
}
