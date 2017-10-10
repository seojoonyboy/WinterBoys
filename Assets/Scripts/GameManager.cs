using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public Ski_PlayerController pC;
    public BoardManager bM;
}
