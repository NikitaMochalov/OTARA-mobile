using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        var dir = (target.position - transform.position).normalized;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }

        var dirL = Quaternion.AngleAxis(-25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirL.y = transform.forward.y;

        var dirR = Quaternion.AngleAxis(25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirR.y = transform.forward.y;

        if (Physics.Raycast(transform.position, dirL, out hit, 20))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
                Debug.DrawRay(transform.position, dirL, Color.blue);
            }
        }

        if (Physics.Raycast(transform.position, dirR, out hit, 20))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
                Debug.DrawRay(transform.position, dirR, Color.blue);
            }
        }
        var rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        transform.position += transform.forward * 5 * Time.deltaTime;
    }
}
