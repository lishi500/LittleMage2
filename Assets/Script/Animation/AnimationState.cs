using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    NONE,
    IDLE,
    WALK,
    RUN,
    FIGHT_STANCE,
    ATTACK1,
    ATTACK2,
    ATTACK3,
    ATTACK,
    CAST,
    CHANNELING,
    CHANNELING_LOOP,
    DEFENCE1,
    GET_HIT,
    DIE,
    
    // Dragon state
    FLY,
    PLAY,

    // Parameter
    RANDOM_100,
    FLY_PROGRESS,
    SKILL_ANIMATION

}
