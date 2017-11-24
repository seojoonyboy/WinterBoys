using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downhill_ItemType : MonoBehaviour {
    public Downhill_itemType type;
}


public enum Downhill_itemType {
    NORMAL,
    BOOST,
    SPEED_REDUCE,
    SPEED_ZERO,
    REVERSE_ROTATE,
    ROTATE_INCREASE,
    ROTATE_REDUCE,
    POINT
}