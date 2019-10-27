using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoAming : MonoBehaviour
{
    PrTopDownCharController charController;
    PrTopDownCharInventory Inventory;

    PlayerController playerController;
    PlayerMovementController playerMovementController;
    Player player;

    [HideInInspector]
    public GameObject currentTarget;
    private float AutoAttackDistance;
    public float rotationSpeed = 0.15f;
    int layer_mask;

    [HideInInspector]
    private GameObject attackIndicatorHolder;
    [HideInInspector]
    private GameObject attackIndicator;
    private PrafabHolder prafabHolder;

    public float updateFrequency = 0.1f;
    private float updateTime;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<PrTopDownCharController>();
        Inventory = GetComponent<PrTopDownCharInventory>();
        playerController = GetComponent<PlayerController>();
        playerMovementController = gameObject.GetComponentInParent<PlayerMovementController>();
        player = GetComponent<Player>();
        AutoAttackDistance = GetComponent<Role>().attackDistance;
        layer_mask = LayerMask.GetMask("Enemy", "Wall");
        currentTarget = null;

        GameObject gameManager = GameObject.FindGameObjectWithTag("GameController");
        prafabHolder = gameManager.GetComponent<PrafabHolder>();
        RecreatorIndicator();
       
    }

    // Update is called once per frame
    void Update()
    {
        updateTime += Time.deltaTime;

        if (playerMovementController._isMoving && updateTime >= updateFrequency)
        {
            GameObject nextTarget = GetNextTarget();
            CompareAndReplaceTarget(nextTarget);
        }

        if (!CanHitTarget(currentTarget)) {
            //Debug.Log("CanHitTarget " + CanHitTarget(currentTarget));
            GameObject nextTarget = GetNextTarget();
            CompareAndReplaceTarget(nextTarget);
        }

        if (currentTarget == null) {
            DisableIndicator();
        }
        //if (playerMovementController._isMoving) { // channelling
        //    currentTarget = null;
        //}

        //if (NoTargetOrDied(currentTarget) && updateTime >= updateFrequency) {
        //    updateTime = 0;
        //    GameObject[] allEnemies = GetAllEnemies();
        //    GameObject[] visiableEnemies = GetVisiableEnemies(allEnemies);

        //    if (!CanHitTarget(currentTarget))
        //    {
        //        currentTarget = GetNearestAttackableEnemy(visiableEnemies);
        //    }

        //    // if no visiable game object
        //    if (currentTarget == null) {
        //        currentTarget = GetNearestAttackableEnemy(GetHiddenEnemys(allEnemies, visiableEnemies));
        //    }
        //}
    }
    private void CompareAndReplaceTarget(GameObject nextTarget) {
        if (nextTarget != null)
        {
            if (currentTarget == null || currentTarget.GetInstanceID() != nextTarget.GetInstanceID())
            {
                currentTarget = nextTarget;
                updateTime = 0;
                EnableIndicator();
            }
        }
    }
    private void DisableIndicator() {
        if (attackIndicator != null) {
            attackIndicator.SetActive(false);
        }
    }

    private void EnableIndicator()
    {
        if (currentTarget != null) {
            RecreatorIndicator();
            attackIndicatorHolder.transform.position = currentTarget.transform.position;
            attackIndicatorHolder.transform.SetParent(currentTarget.transform);
            attackIndicator.SetActive(true);
        }
    }

    private void RecreatorIndicator() {
        if (attackIndicatorHolder == null) {
            attackIndicatorHolder = Instantiate(prafabHolder.attackTargetIndicator);
            attackIndicator = attackIndicatorHolder.transform.Find("indicator").gameObject;
            attackIndicator.SetActive(false);
        }
    }

    private GameObject GetNextTarget() {
        GameObject[] allEnemies = GetAllEnemies();
        GameObject[] visiableEnemies = GetVisiableEnemies(allEnemies);

        GameObject target = GetNearestAttackableEnemy(visiableEnemies);
        if (target == null)
        {
            target = GetNearestAttackableEnemy(GetHiddenEnemys(allEnemies, visiableEnemies));
        }

        return target;
    }

    public GameObject GetNearestAttackableEnemy(GameObject[] enemies) {
        if (enemies.Length == 0)
        {
            //Debug.Log("No visiable enemy found");
            return null;
        }

        float shortestDistance = float.MaxValue;
        int nearstIndex = -1;

        for (int i = 0; i < enemies.Length; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, enemies[i].transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearstIndex = i;
            }
        }


        return enemies[nearstIndex];
    }

    private GameObject[] GetAllEnemies() {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }

    private GameObject[] GetVisiableEnemies(GameObject[] enemies) {
        List<GameObject> visiableEnemies = new List<GameObject>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (CanHitTarget(enemies[i]))
            {
                visiableEnemies.Add(enemies[i]);
            }
        }

        return visiableEnemies.ToArray();
    }

    private GameObject[] GetHiddenEnemys(GameObject[] allEnemies, GameObject[] visiableEnemies) {
        return allEnemies.Except(visiableEnemies).ToArray<GameObject>();
    }

    private GameObject[] GetVisiableEnemies()
    {
        return GetVisiableEnemies(GetAllEnemies());
    }


    private void AutoAmingTarget()
    {
        Vector3 playerSensorPos = gameObject.transform.position + new Vector3(0f, 1.6f, 0f);
        Vector3 targetDir = currentTarget.transform.position + new Vector3(0f, 1.6f, 0f) - playerSensorPos;

        float angle = Vector3.Angle(targetDir, transform.forward);
        if (angle < 30) {
            Inventory.AimingStand();
        }
    }

    public bool HasAttackableTarget()
    {
        return currentTarget != null && currentTarget.GetComponent<Role>().isAlive;
    }
    private bool NoTargetOrDied(GameObject target) {
        return target == null || !target.GetComponent<Role>().isAlive;
    }

    private void AttackTarget()
    {
        //Debug.Log("Attack AttackTarget");

        //playerController.AttackPosition(currentTarget.transform);
    }


    /*   private void AutoReload() {
           if (Inventory.GetActiveWeaponBulletsLeft() == 0)
           {
               Inventory.WeaponReload();
           }
       }
    */

    private bool CanHitTarget(GameObject target) {
        if (NoTargetOrDied(target)) {
            return false;
        }

        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);
        if (distance > AutoAttackDistance) {
            //Debug.Log("exceed limit");
            return false;
        }


        Vector3 playerSensorPos = gameObject.transform.position + new Vector3(0f, 0.5f, 0f);
        Vector3 targetDir = target.transform.position + new Vector3(0f, 0.5f, 0f) - playerSensorPos;

        RaycastHit hit;
        //Debug.DrawRay(playerSensorPos, targetDir, Color.blue);
        if (Physics.Raycast(playerSensorPos, targetDir, out hit, Mathf.Infinity, layer_mask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                //Debug.DrawRay(playerSensorPos, targetDir, Color.yellow, 1f);
                return true;
            }

        }
       
        return false;
    }

}
