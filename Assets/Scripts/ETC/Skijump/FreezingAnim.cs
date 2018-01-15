using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FreezingAnim : MonoBehaviour {
    private SkeletonAnimation anim;
    private Type currentType;

    private void Awake() {
        anim = GetComponent<SkeletonAnimation>();
        currentType = Type.NONE;
    }

    public void setAnim(Type type) {
        if(currentType == type) { return; }

        gameObject.SetActive(true);
        
        //Debug.Log(type);
        switch (type) {
            case Type.FREEZING:
                anim.timeScale = 2f;
                anim.AnimationName = "freezing";
                break;
            case Type.STUNNING:
                anim.timeScale = 1f;
                anim.AnimationName = "freezing_stun";
                break;
            case Type.NONE:
                gameObject.SetActive(false);
                break;
        }
        currentType = type;
    }

    public enum Type {
        NONE,
        FREEZING,
        STUNNING
    }
}
