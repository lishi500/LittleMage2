using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
   IDLE,
   PATROL,
   APPROACHING,
   ATTACK,
   CAST,
   FLEE,
   REACT_HIT,

   // Pet state
   PLAY,
   GUARD,
   CREATE_ORB,
   REST_RECOVER,
   REVIVE,
   FOLLOW,
   FLY_TO

}
