using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningLink : MonoBehaviour
{
    // default line renderer
    public LineRenderer m_LineRenderer;

    // LineRenderer vertex list
    Vector3[] m_LinePositions;
    // vertex list with noise added
    Vector3[] m_NoiseLinePositions;

    // distance between vertices
    public float m_DistancePerSegment = 1;
    // if it changes, the line renderer is reset
    private float m_LastDistanceValue;
    //
    private float m_NoiseDistance;

    // start and end transforms
    public Transform m_StartPoint;
    public Transform m_EndPoint;

    public float yOffSet;
    // calculates number of segmetns based on m_DistancePerSegment and distance between start/end
    private int m_NumSegments;

    // decay speed in meters per second
    public float m_DecaySpeed = 30;
    // current decay distance
    private float m_DecayDistance = 0;
    bool m_IsDecaying = false;

    // resets the line renderer and vertices
    void Start()
    {
        transform.position = Vector3.zero;
        if (m_StartPoint != null && m_EndPoint != null)
        {
            //SetStartEndPoints(new Vector3(0, 0, -5), new Vector3(0, 0, 5));
            SetStartEndPoints(m_StartPoint.position + new Vector3(0, yOffSet, 0), m_EndPoint.position + new Vector3(0, yOffSet, 0));

            InvokeRepeating("RandomizeLine", 0, .03f);
        }
    }

    public void SetStartEndPoints(Vector3 start, Vector3 end)
    {
        // remove all invokes
        CancelInvoke();

        // makes sure min distance is not less than .01
        if (m_DistancePerSegment <= .01f)
            m_DistancePerSegment = .01f;

        // calculate direction of the lightning and distance
        Vector3 dir = end - start;
        dir.Normalize();

        float distance = Vector3.Distance(start, end);

        // calucalte num segments
        m_NumSegments = Mathf.FloorToInt(distance / m_DistancePerSegment);

        // reset all positions from end to start
        // this is done so decay is a simple change in size instead of shifting the vertices
        m_LinePositions = new Vector3[m_NumSegments];
        m_NoiseLinePositions = new Vector3[m_NumSegments];

        m_LinePositions[0] = end;
        m_NoiseLinePositions[0] = end;
        dir *= m_DistancePerSegment;

        for (int i = 1; i < m_NumSegments; ++i)
        {
            m_LinePositions[i] = -i * dir + end;
            m_NoiseLinePositions[i] = -i * dir + end;
        }

        m_LinePositions[m_LinePositions.Length - 1] = start;
        m_NoiseLinePositions[m_LinePositions.Length - 1] = start;

        // set positions on line renderer
        m_LineRenderer.positionCount = m_NumSegments;
        m_LineRenderer.SetPositions(m_NoiseLinePositions);

        // calculate the amount of noise as a function of distance per segment
        m_NoiseDistance = m_DistancePerSegment / 2f;
        m_LastDistanceValue = m_DistancePerSegment;
    }

    // watched for hotkeys to decay or reset
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_DecayDistance = 0;
        //    m_IsDecaying = true;
        //    InvokeRepeating("Decay", 0, .001f);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    m_IsDecaying = false;
        //}

        if (m_IsDecaying == false)
        {
            Start();
        }
    }

    // line renderer vertices are removed from start to end
    void Decay()
    {
        m_DecayDistance += Time.deltaTime * m_DecaySpeed;
        int lostSegments = Mathf.FloorToInt(m_DecayDistance / m_DistancePerSegment);
        int remainingSegments = m_NumSegments - Mathf.Clamp(lostSegments, 0, m_NumSegments);
        m_LineRenderer.positionCount = remainingSegments;
        
        if (remainingSegments == 0)
        {
            CancelInvoke();
        }
    }

    // applies noise  to the line renderer
    void RandomizeLine()
    {
        // resets if the distance per segment is changed
        if (m_DistancePerSegment != m_LastDistanceValue)
        {
            Start();
        }

        for (int i = 1; i < m_LinePositions.Length - 1; ++i)
        {
            m_NoiseLinePositions[i] = m_LinePositions[i] + Random.onUnitSphere * m_NoiseDistance;
        }
        m_LineRenderer.SetPositions(m_NoiseLinePositions);
    }
}
