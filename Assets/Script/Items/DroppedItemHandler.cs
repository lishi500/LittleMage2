using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SensorToolkit;
using VisCircle;

public class DroppedItemHandler : MonoBehaviour
{
    public bool autoTrackPlayer = true;
    public bool touchToCollect = true;
    public bool movable = true;

    private AbstractItemHandler dropItem;
    
    private Player player;
    private Transform targetReceivePoint;

    private float MinModifier = 12;
    private float MaxModifier = 17;

    private float COLLECTING_DISTANCE = 5f;
    private float COLLECT_FINAL_DISTANCE = 2f;

    Vector3 _velocity = Vector3.zero;

    private RangeSensor sensor;
    public enum DroppedState {
        Dropping,
        Dropped,
        Collecting,
        Collected
    }
    public DroppedState state;

    public void OnTargetEnter() {
        if (state != DroppedState.Collecting && state != DroppedState.Collected) {
            //Debug.Log("OnTargetEnter ");
            foreach (GameObject obj in GetSensor().DetectedObjects)
            {
                if (ProcessTargetEnter(obj))
                {
                    return;
                }
            }
        }
    }

    public void OnTargetLeave() {
        if (GetSensor().DetectedObjects.Count() == 0) {
            state = DroppedState.Dropped;
        }
    }

    public bool ProcessTargetEnter(GameObject obj)
    {
        //Debug.Log("ProcessTargetEnter " + obj.name);
        if (touchToCollect) {
            if (GetItemHandler().type == ItemType.Consumable)
            {
                Role otherRole = obj.gameObject.GetComponent<Role>();

                if (otherRole != null)
                {
                    ConsumableItemHandler consumableItem = (ConsumableItemHandler) GetItemHandler();
                    if (consumableItem.exceptedTriggerRole.Contains(otherRole.roleType))
                    {
                        consumableItem.triggerTarget = otherRole;
                        targetReceivePoint = otherRole.GetShootPoint(ShootPointPosition.RECEIVE);
                        TrackPlayer();
                        return true;
                    }
                }
            }
            else {
                TrackPlayer();
                return true;
            }
        }
        return false;
    }

    public void Bounce() {
        state = DroppedState.Dropping;
    }

    public void Collect() {
        if (GetItemHandler() != null) {
            GetItemHandler().CollectItem();
        }
        state = DroppedState.Collected;
    }

    private void AssignToPlayer() {

    }

    public void TrackPlayer() {
        state = DroppedState.Collecting;
        touchToCollect = true;
    }

    private void TrackingInProgress() {
        if (movable) {
            transform.position = Vector3.SmoothDamp(transform.position, targetReceivePoint.transform.position, ref _velocity,
           Time.deltaTime * Random.Range(MinModifier, MaxModifier));
        }

        if (Vector3.Distance(transform.position, targetReceivePoint.transform.position) < COLLECT_FINAL_DISTANCE)
        {
            Collect();
        }
    }

    private AbstractItemHandler GetItemHandler() {
        if (dropItem == null) {
            dropItem = transform.GetComponentInChildren<AbstractItemHandler>();
        }

        return dropItem;
    }

    private RangeSensor GetSensor() {
        if (sensor == null) {
            sensor = GetComponent<RangeSensor>();
        }
        return sensor;
    }
    private void DestroyItem() {
        Destroy(this.gameObject);
    }

    void Start()
    {
        player = Finder.Instance.GetPlayer() ;
        targetReceivePoint = player.GetShootPoint(ShootPointPosition.RECEIVE);
        sensor = GetSensor();
        
        Physics.IgnoreLayerCollision(LayerMapping.Item, LayerMapping.Enemy);
        Physics.IgnoreLayerCollision(LayerMapping.Item, LayerMapping.Item);
        Physics.IgnoreLayerCollision(LayerMapping.Item, LayerMapping.Bullet);
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.2f, 0.2f), 1, Random.Range(-0.2f, 0.2f)) * 400f);

     
        StartCoroutine(DroppingItem());
        StartCoroutine(StopBounce());
    }

    private IEnumerator DroppingItem() {
        yield return new WaitForSeconds(1f);

        state = DroppedState.Dropped;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (autoTrackPlayer) {
            TrackPlayer();
        }
    }
    private IEnumerator StopBounce()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }


    // Update is called once per frame
    void Update()
    {
        if (state == DroppedState.Collecting) {
            TrackingInProgress();
        } else if (state == DroppedState.Collected){
            DestroyItem();
        }
    }
}
