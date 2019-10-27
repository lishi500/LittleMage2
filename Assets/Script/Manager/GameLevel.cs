using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "LeveMap/level", order = 1)]
public class GameLevel : ScriptableObject
{
    public int levelNo;
    public int subLevel;
    public int numOfWave;
    public GameEnemySpawn[] spawnInfo;
    //public List<GameObject> wave1;
    //public List<GameObject> wave2;
    //public List<GameObject> wave3;

}

[Serializable]
public class GameEnemySpawn
{
    public List<GameObject> enemies;
}
