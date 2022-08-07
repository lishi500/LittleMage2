using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrailManager : MonoBehaviour
{
    public void AddTrail(GameObject trailPrafab, TrailPointType trailPointType) {
        List<TrailPoint> trailPoints = GetTrailPointsByTypes(new TrailPointType[] { trailPointType });

        foreach (TrailPoint trail in trailPoints)
        {
            trail.AddTrail(trailPrafab);
        }
    }
    public void TrunOnTrailByTypes(TrailPointType[] trailPointType) {
        List<TrailPoint> trailPoints = GetTrailPointsByTypes(trailPointType);

        foreach (TrailPoint trail in trailPoints) {
            trail.StartTrail();
        }
    }

    public void TrunOffTrailByTypes(TrailPointType[] trailPointType)
    {
        List<TrailPoint> trailPoints = GetTrailPointsByTypes(trailPointType);

        foreach (TrailPoint trail in trailPoints)
        {
            trail.StopTrail();
        }
    }

    private List<TrailPoint> GetTrailPointsByTypes(TrailPointType[] trailPointType) {
        TrailPoint[] allTrailPoints = GetAllTrailPoints();
        return allTrailPoints.Where(trailPoint => {
            return trailPointType.Contains(trailPoint.trailType);
        }).ToList();
    }

    private TrailPoint[] GetAllTrailPoints() {
        return GetComponentsInChildren<TrailPoint>();
    }
}
