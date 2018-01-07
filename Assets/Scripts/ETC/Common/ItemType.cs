public class ItemType {
    //downhill
    public enum DH {        //움직이는 방해물은 ENEMY, 고정되어있는 방해물은 OBSTACLE
        POINT,
        BOOSTING_HILL,
        ANTI_SPEED_HILL,
        ENEMY_BEAR,
        ENEMY_BUGS,
        TREE,
        OBSTACLE_POLL,
        OBSTACLE_OIL,
        MONEY,
        TIME
    }

    //skijump
    public enum SJ {
        POINT,
        WH_BIRD,
        BL_BIRD,
        DK_CLOUD,
        BALLOON,
        THUNDER_CLOUD,
        MONEY,
        TIME
    }

    //skelton
    public enum ST {
        POINT,
        BOOST,
        ICE,
        BUGS,
        BOND,
        OIL,
        MONEY,
        WATCH
    }
}
