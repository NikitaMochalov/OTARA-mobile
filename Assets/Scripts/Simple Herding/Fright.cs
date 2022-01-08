using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fright : MonoBehaviour
{
    [Range(0, 10)]
    public float RunSpeed = 4f;

    SheepController sheepController;

    private void Start()
    {
        sheepController = GetComponent<SheepController>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = -(sheepController.dogPosition - transform.position).normalized;
        dir.y = transform.forward.y;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, sheepController.DstToObstacles, sheepController.obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }
        if (Physics.SphereCast(transform.position, 1, transform.forward, out hit, sheepController.DstBetweenSheeps, sheepController.sheepMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 25;
            }
        }

        var dirL = Quaternion.AngleAxis(-25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirL.y = transform.forward.y;

        var dirR = Quaternion.AngleAxis(25, transform.up) * transform.forward * (1 + transform.localScale.x / 2);
        dirR.y = transform.forward.y;

        if (Physics.Raycast(transform.position, dirL, out hit, sheepController.DstToObstacles, sheepController.obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }
        //if (Physics.SphereCast(transform.position, 1, dirL, out hit, sheepController.DstBetweenSheeps, sheepController.sheepMask))
        //{
        //    if (hit.transform != transform)
        //    {
        //        dir += hit.normal * 50;
        //    }
        //}

        if (Physics.Raycast(transform.position, dirR, out hit, sheepController.DstToObstacles, sheepController.obstacleMask))
        {
            if (hit.transform != transform)
            {
                dir += hit.normal * 50;
            }
        }
        //if (Physics.SphereCast(transform.position, 1, dirR, out hit, sheepController.DstBetweenSheeps, sheepController.sheepMask))
        //{
        //    if (hit.transform != transform)
        //    {
        //        dir += hit.normal * 50;
        //    }
        //}

        var rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        sheepController.controller.SimpleMove(forward * RunSpeed);
    }
}
