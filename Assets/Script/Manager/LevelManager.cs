using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public int level;
    public int subLevel;
    private bool levelStarted;

    public GameLevel currentLevelData;
    public GameObject currentLevelMap;
    public List<GameObject> currentEnemies;
    private int[] levelCount = new int[] { 1 };

    [SerializeField]
    private StringGameObjectDictionary levelStore = StringGameObjectDictionary.New<StringGameObjectDictionary>();
    private Dictionary<string, GameObject> levelStringObject
    {
        get { return levelStore.dictionary; }
    }


    public bool IsLevelComplete() {
        if (!levelStarted) {
            return false;
        }

        foreach (GameObject enemy in currentEnemies) {
            Enemy enemyRole = enemy.GetComponent<Enemy>();
            if (enemyRole != null) {
                if (enemyRole.isAlive) {
                    return false;
                }
            }
        }

        return true;
    }

    public void StartLevel() {
        //Debug.Log("Start Level： " + level + "-" + subLevel);
        levelStarted = true;
    }
    public void EndLevel() {
        Debug.Log("End Level： " + level + "-" + subLevel);

        levelStarted = false;
    }

  
    public void SpawnEnemies() {
        Debug.Log("SpawnEnemies: ");

       int numberOfSpawnPoint = currentLevelData.spawnInfo.Length;
        Transform[] spawnPoints = GetSpawnPoints();

        if (spawnPoints == null || spawnPoints.Length == 0) {
            Debug.LogError("Cannot find Spawn points");
        }

        for (int i = 0; i < numberOfSpawnPoint; i++) {
            List<GameObject> enemiesPrafabs = currentLevelData.spawnInfo[i].enemies;
            foreach (GameObject enemyPrafab in enemiesPrafabs) {
                GameObject newEnemy = GameObject.Instantiate(enemyPrafab, spawnPoints[i].position, Quaternion.identity);
                AdjuestPosition(newEnemy);

                currentEnemies.Add(newEnemy);
                //Debug.Log("    " + newEnemy.name);
            }
        }
    }
    private void AdjuestPosition(GameObject obj) {

        CapsuleCollider capsuleCollider = obj.GetComponent<CapsuleCollider>();
        Vector3 center = obj.transform.position;
        Collider[] colliders = Physics.OverlapCapsule(center, center, capsuleCollider.radius);
        bool touched = colliders.Length > 1;

        int count = 0;
        int aa = obj.GetInstanceID();
        while (touched && count < 15) {
            Debug.Log("AdjuestPosition for " + obj.name + " = " + count++);
            foreach (Collider collider in colliders) {
                if (collider.transform == obj.transform) {
                    continue;
                }
                int bb = collider.gameObject.GetInstanceID();
                Vector3 centerPoint = collider.bounds.center;
                float expectedDistance = capsuleCollider.radius + Vector3.Distance(collider.bounds.extents, Vector3.zero);
                float currentDistance = Vector3.Distance(center, collider.bounds.center);
                float moveDistance = expectedDistance - currentDistance;

                obj.transform.position = obj.transform.position + (new Vector3(1, 0, 1) * moveDistance);
                capsuleCollider = obj.GetComponent<CapsuleCollider>();
                center = obj.transform.position;
            }

            center = obj.transform.position;
            colliders = Physics.OverlapCapsule(center, center, capsuleCollider.radius);
            touched = colliders.Length > 1;
        }

        
    }
    public void LoadNextSubLevel() {
        // load next
        int maxSubLevel = levelCount[level - 1];
        if (subLevel < maxSubLevel)
        {
            subLevel += 1;

            GameObject levelObj = GetLevelPrafab(level, subLevel);
            GameLevel gameLevel = LoadGameLevel(level, subLevel);
            currentLevelMap = GameObject.Instantiate(levelObj);
            currentLevelData = gameLevel;
            StartLevel();
            //SpawnEnemies();
        }
        else {
          // finish game for current Level
        }
        
    }

    void Start()
    {
        currentEnemies = new List<GameObject>();
        LoadNextSubLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // private -------------------------
    private Transform[] GetSpawnPoints()
    {
        Transform[] points = currentLevelMap.transform.Find("spanwPoints").GetComponentsInChildren<Transform>();
        return points.Skip(1).ToArray<Transform>();
    }
    private GameLevel LoadGameLevel(int level, int subLevel) {
        GameLevel gameLevel = Resources.Load<GameLevel>("Data/Level/Level" + level + "-" + subLevel);
        return gameLevel;
    }
    private GameObject GetLevelPrafab(int level, int subLevel) {
        GameObject value;

        bool exist = levelStore.dictionary.TryGetValue(level + "-" + subLevel, out value);
        return exist ? value : null;
    }
}
