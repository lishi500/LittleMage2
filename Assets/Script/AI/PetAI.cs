using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ECM.Components;

public class PetAI : BaseAI
{
    protected Role currentTarget;
    protected Pet pet;
    protected RangeSensor rangeSensor;

    protected string[] EnemyTags = new string[] { "Enemy" };

    protected float minHoldTargetTime = 2f;
    protected float targetHoldTimmer = 0f;
    protected Player player {
        get { return Finder.Instance.GetPlayer(); }
    }
    protected float petFollowDistance = 10f;
    protected DragonEventHelper dragonEventHelper {
        get { return (DragonEventHelper) eventHelper; }
    }

    protected bool isSafeState = true;
    protected bool isFlying = false;
    protected bool orbSkillReady = false;
   

    private float previousHeight;
    public float flySpeed = 0.05f;

    private Vector3 startPosition;
    private Vector3 landingPoint;

    const float minFlyDistance = 10f;
    const float flyStopDistance = 3f;

    private AIState[] cannotCast = { AIState.ATTACK, AIState.CAST, AIState.REACT_HIT, AIState.FLEE,
                                     AIState.GUARD, AIState.CREATE_ORB, AIState.REST_RECOVER, AIState.REVIVE };

    protected SkillType[] ACTIVE_SKILL_TYPES = new SkillType[] { SkillType.ACTIVE, SkillType.ENHANCEMENT, SkillType.GROUND };


    public override void SwitchState()
    {
    }

    public override void UpdateBehaviour()
    {
    }

    public override void InProgressMonitor()
    {
    }

    public override GameObject GetTargetObject() {
        //Debug.Log("GetTargetObject PetAI");

        if (currentTarget == null)
        {
            currentTarget = SwitchTarget();
        }
        else if (!currentTarget.isAlive
            || !aiUtils.CanSeeObject(gameObject, currentTarget.gameObject)
            || (pet.isRange && aiUtils.DistanceBetweenObject(currentTarget.gameObject, gameObject) > pet.attackDistance)
            || (!pet.isRange && aiUtils.DistanceBetweenObject(currentTarget.gameObject, gameObject) > pet.MaxChaseDistance))
        {
            currentTarget = SwitchTarget();
        }

        return currentTarget == null ? null : currentTarget.gameObject;
    }

    public Role SwitchTarget() {
        Role playerTargetRole = GetPlayerCurrentTargetRole();

        if (pet.isRange)
        {
            if (playerTargetRole != null && aiUtils.CanSeeObject(gameObject, playerTargetRole.gameObject))
            {
                return playerTargetRole;
            }
            List<GameObject> visiableObjects = aiUtils.GetVisiableObject(GetSensorEnemy(), gameObject);
            Role nextEnemy = GetNearestSensorEnemy(visiableObjects, player.gameObject);
            if (nextEnemy != null)
            {
                return nextEnemy;
            }
        }
        else {
            Role nextEnemy = GetNearestSensorEnemy(GetSensorEnemy(), player.gameObject);
            if (nextEnemy != null) {
                return nextEnemy;
            }
        }

        return playerTargetRole;
    }

    public bool CanCast() {
        return !cannotCast.Contains(currentState);
    }

    public virtual bool CanAttack()
    {
        return currentTarget != null &&
            aiUtils.CanAttack(gameObject, currentTarget.gameObject, pet.attackDistance, true);
    }

    protected void PlayAction() {
        if (progress == AIProgress.INIT)
        {
            progress = AIProgress.RUNNING;
            animationController.animationState = AnimationState.NONE;
            animationController.AddAutoClearState(AnimationState.PLAY, true);
            dragonEventHelper.notifyPlayEnd += PlayActionEnd;
        }
    }
    public void PlayActionEnd() {
        if (currentState == AIState.PLAY) {
            progress = AIProgress.END;
            dragonEventHelper.notifyPlayEnd -= PlayActionEnd;
        }
    }
    public void CastSkillAction() {
        if (progress == AIProgress.INIT) {
            //Debug.Log("CastSkillAction " + currentSkill.name);
            progress = AIProgress.RUNNING;
            List<SkillBehaviorType> behaviors = currentSkill.skillData.skillBehaviorTypes;

            Transform skillTarget = CalculateSkillTarget();
            CastSkill(currentSkill, skillTarget);

        }
        if (progress == AIProgress.RUNNING) {

        }

    }

    private Transform CalculateSkillTarget() {
        GameObject skillTargetObject = currentSkill.GetCustomTarget();
        if (skillTargetObject == null)
        {
            if (IsOffensiveSkill(currentSkill))
            {
                skillTargetObject = GetTargetObject();
            }
            else if (IsDefensiveSkill(currentSkill))
            {
                if (currentSkill.isTargetAllies)
                {
                    skillTargetObject = player.gameObject;
                }
                else
                {
                    skillTargetObject = gameObject;
                }
            }
            else
            {
                skillTargetObject = GetTargetObject();
            }
        }

        return skillTargetObject.transform;
    }

    protected bool ShouldCast(Skill skill)
    {
        bool shouldCast = false;
        if (skill == null) {
            return false;
        }

        if (skill.type == SkillType.ACTIVE) {
            if (EnemyExist(skill.castRange))
            {
                shouldCast = true;
            }
        }

        if (skill.type == SkillType.ENHANCEMENT) {

        }

        if (skill.type == SkillType.GROUND) {
            if (EnemyExist())
            {
                shouldCast = true;
            }
        }

        return shouldCast;
    }

    protected bool EnemyExist(float range = float.MaxValue) {
        GameObject[] enemies = GetAllEnemies();
        return enemies.Where(enemyObj =>
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            float distance = aiUtils.DistanceBetweenObject(gameObject, enemyObj);
            return enemy != null && enemy.isAlive && range > distance;
        }).Count() > 0;
    }

    protected bool IsOffensiveSkill(Skill skill) {
        List<SkillBehaviorType> behaviors = currentSkill.skillData.skillBehaviorTypes;
        if (behaviors != null &&
            (behaviors.Contains(SkillBehaviorType.DAMAGE)
            || behaviors.Contains(SkillBehaviorType.CONTROL)
            || behaviors.Contains(SkillBehaviorType.SUMMON)
            || behaviors.Contains(SkillBehaviorType.CHARGE)
         )) { 
            return true;
        }

        return false;
    }

    protected bool IsDefensiveSkill(Skill skill)
    {
        List<SkillBehaviorType> behaviors = currentSkill.skillData.skillBehaviorTypes;
        if (behaviors != null &&
            (behaviors.Contains(SkillBehaviorType.HEAL)
            || behaviors.Contains(SkillBehaviorType.ENHANCE)
            || behaviors.Contains(SkillBehaviorType.SHIELD)
            || behaviors.Contains(SkillBehaviorType.BLOCK_SHIELD)
            || behaviors.Contains(SkillBehaviorType.REVIVE)
            || behaviors.Contains(SkillBehaviorType.DASH)
        ))
        {
            return true;
        }
        return false;
    }

    private GameObject[] GetAllEnemies()
    {
        return GameObject.FindGameObjectsWithTag(TagMapping.Enemy.ToString());
    }


    //protected void FlyAction() {

    //    if (progress == AIProgress.INIT)
    //    {
    //        previousHeight = transform.position.y;
    //        animationController.animationState = AnimationState.FLY;
    //        //pet.GetComponent<CharacterMovement>().useGravity = false;

    //        bezierPoints = aiUtils.GenerateStandardAncherBezierPoint(transform.position, GetNextFlyWayPoint().position);

    //        progress = AIProgress.RUNNING;
    //    }
    //    if (progress == AIProgress.RUNNING && GetNextFlyWayPoint() != null)
    //    {
    //        Vector3 nextPoint = aiUtils.CalculateBezierPoint(distancePercentage, bezierPoints);
    //        transform.position = nextPoint;
    //        if (distancePercentage <= 1f) //check every frame to see if the end of the curve has been reached
    //        {
    //            distancePercentage += Time.deltaTime * flySpeed + (0.5f - Mathf.Abs(0.5f - distancePercentage)) * 0.02f;
    //            transform.LookAt(aiUtils.CalculateBezierPoint(distancePercentage, bezierPoints)); //keeps the ship facing in the direction that it is flying
    //        }
    //        else {
    //            progress = AIProgress.END;
    //            //pet.GetComponent<CharacterMovement>().useGravity = true;
    //            animationController.animationState = AnimationState.RUN;
    //        }
    //    }
    //    else
    //    {
    //        progress = AIProgress.END;
    //    }

    //}
    float distancePercentage;

    public void FlyToTarget(Transform target, float stopDistance = flyStopDistance) {
        if (distancePercentage < 0.8)
        {
            landingPoint = aiUtils.DistanceToPosition(transform.position, player.transform.position, flyStopDistance);
            landingPoint.y = 0.3f;
        }
        FlyTo(startPosition, landingPoint);
    }

    public float FlyTo(Vector3 start, Vector3 targetPos) {
        if (targetPos != null) {
            Vector3[] bezierPoints = aiUtils.GenerateStandardAncherBezierPoint(start, targetPos);
            Vector3 nextPoint = aiUtils.CalculateBezierPoint(distancePercentage, bezierPoints);

            transform.position = nextPoint;            transform.position = nextPoint;

            if (distancePercentage <= 1f) //check every frame to see if the end of the curve has been reached
            {
                distancePercentage += Time.deltaTime * flySpeed + (0.5f - Mathf.Abs(0.5f - distancePercentage)) * 0.02f;
                transform.LookAt(aiUtils.CalculateBezierPoint(distancePercentage, bezierPoints)); //keeps the ship facing in the direction that it is flying
                // TODO LookAt, between landing point and target
                return distancePercentage;
            }
        }

        return 1;
    }

    protected void FollowAction() {

        bool arrived = false;
        if (progress == AIProgress.INIT)
        {
            progress = AIProgress.RUNNING;
            if (aiUtils.DistanceBetweenObject(gameObject, player.gameObject) > minFlyDistance) {
                InitialFly();
            }
        }

        if (progress == AIProgress.RUNNING)
        {
            float distance = aiUtils.DistanceBetweenObject(gameObject, player.gameObject);

            if (isFlying)
            {
                animationController.animationState = AnimationState.FLY;

                FlyToTarget(player.transform);
                if (distancePercentage >= 1) {
                    isFlying = false;
                    transform.LookAt(player.transform);
                }
                animationController.SetFloat(AnimationState.FLY_PROGRESS, distancePercentage);
            }
            else {
                animationController.animationState = AnimationState.RUN;
                arrived = ChasingTarget(player.transform, 3f);
                if (distance > minFlyDistance) {
                    InitialFly();
                }
            }
            //Debug.Log("FollowAction Running " + arrived);
        }

        if (arrived)
        {
            Debug.Log("FollowAction END " + arrived);
            progress = AIProgress.END;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(landingPoint, 1f);
    }
    private void InitialFly() {
        isFlying = true;
        distancePercentage = 0;
        startPosition = transform.position;
    }

    public void Sleep() { // dead, need to sleep to revive
        // immu, bullet pass though
    }

    public void Revive() {

    }

    public void PlayCute() {

    }

    public AIState SwitchToSafeState() {
        AIState[] safeState = { AIState.PLAY, AIState.PATROL, AIState.FOLLOW };
        if (aiUtils.DistanceBetweenObject(gameObject, player.gameObject) > petFollowDistance) {
            return AIState.FOLLOW;
        }
        System.Random rnd = new System.Random();
        int next = rnd.Next(0, safeState.Length);
        isSafeState = true;

        return safeState[next];
    }

    public void SwitchFromSafeState()
    {
        GetTargetObject();
        isSafeState = false;
        currentState = currentTarget == null ? SwitchToSafeState() : AIState.APPROACHING;
        //Debug.Log("SwitchFromSafeState -> " + currentState);
    }

    private Role GetPlayerCurrentTargetRole() {
        if (player.target != null) {
            return player.target.GetComponent<Role>();
        }
        return null;
    }

    private List<GameObject> GetSensorEnemy() {
        if (rangeSensor.DetectedObjects != null && rangeSensor.DetectedObjects.Count() > 0)
        {
            return rangeSensor.DetectedObjects.Where(obj => {
                Role role = obj.GetComponent<Role>();
                return role != null && role.isAlive;
            }).ToList();
        }

        return new List<GameObject>() ;
    }
    private Role GetNearestSensorEnemy(List<GameObject> enemies, GameObject sourceTarget) {
        if (enemies.Count > 0)
        {
            GameObject nearest = aiUtils.GetNearestObject(enemies, sourceTarget);
            return nearest.GetComponent<Role>();
        }

        return null;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        updateFrequency = 0f;
        pet = GetComponent<Pet>();
        rangeSensor = GetComponent<RangeSensor>();
    }

}
