using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public BoardManager bM;
    public Sprite[] players;
}
