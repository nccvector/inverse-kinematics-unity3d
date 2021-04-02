using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    const int numParams = 4;
    public float[] qs = {0, 90, 0, 20};
    float[] qMinLim = {-1000000, 0, -90, 0};
    float[] qMaxLim = {1000000, 180, 90, 20};

    GameObject linkA, linkB, linkC, wrist, clipA, clipB;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        linkA = this.transform.GetChild(0).transform.GetChild(0).gameObject;
        linkB = linkA.transform.GetChild(0).gameObject;
        linkC = linkB.transform.GetChild(0).gameObject;
        wrist = linkC.transform.GetChild(0).gameObject;
        clipA = wrist.transform.GetChild(0).gameObject;
        clipB = wrist.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Checking param limits before applying them
        LimitParams();

        // Updating joint angles based on q params in the loop
        MoveTowardTarget();
    }

    void LimitParams()
    {
        for (int i=0; i<numParams; i++)
        {
            if (qs[i] < qMinLim[i])
                qs[i] = qMinLim[i];
            else if (qs[i] > qMaxLim[i])
                qs[i] = qMaxLim[i];
        }
    }

    void ApplyParamsToJoints()
    {
        linkA.transform.localEulerAngles = new Vector3(
            90,
            0,
            qs[0]
        );

        linkB.transform.localEulerAngles = new Vector3(
            linkB.transform.localEulerAngles.x,
            linkB.transform.localEulerAngles.y,
            qs[1]
        );

        linkC.transform.localEulerAngles = new Vector3(
            linkC.transform.localEulerAngles.x,
            linkC.transform.localEulerAngles.y,
            qs[2]
        );
        // linkA.transform.eulerAngles = new Vector3(0, 0, q4);
    }

    void MoveTowardTarget(float stepSize = 5f)
    {
        float minDistance = Vector3.Distance(wrist.transform.position, target.position);
        float[] closestParams = {qs[0], qs[1], qs[2], qs[3]};
        float[] tempQs = {qs[0], qs[1], qs[2], qs[3]};

        for(int iq0=-1; iq0<2; iq0++)
        {
            qs[0] = tempQs[0] + iq0 * stepSize;
            for(int iq1=-1; iq1<2; iq1++)
            {
                qs[1] = tempQs[1] + iq1 * stepSize;
                for(int iq2=-1; iq2<2; iq2++)
                {
                    qs[2] = tempQs[2] + iq2 * stepSize;
                    qs[3] = 20;

                    // Finding distance to goal
                    ApplyParamsToJoints();
                    float currDistance = Vector3.Distance(wrist.transform.position, target.position);
                    if (currDistance < minDistance)
                    {
                        minDistance = currDistance;
                        closestParams[0] = qs[0];
                        closestParams[1] = qs[1];
                        closestParams[2] = qs[2];
                        closestParams[3] = qs[3]; 
                    }
                }   
            }
        }

        // Applying the closest params to qs
        for (int i=0; i<numParams; i++)
            qs[i] = Mathf.Lerp(tempQs[i], closestParams[i], 10f * Time.deltaTime);

        ApplyParamsToJoints();
    }
}
