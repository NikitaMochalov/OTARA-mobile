using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graze : MonoBehaviour
{
    [Range(0, 10)]
    public float Speed = 1f;
    public MeshRenderer mesh;
    public float EatTime = 1f;

    SheepController sheepController;
    bool goingToTarget;
    //bool eating;
    Vector3 target;
    float distanceToTarget;

    private void Start()
    {
        sheepController = GetComponent<SheepController>();
    }

    void Update()
    {
        //create new graze zone target
        if (!goingToTarget /*&& !eating*/)
        {
            var newTarget = GetRandomPointOnTheGroundInRange(4);
            if (newTarget != Vector3.zero) target = newTarget;

            goingToTarget = true;
        }

        if (goingToTarget)
        {
            Vector3 xzTargetPos = target;
            xzTargetPos.y = transform.position.y;
            Vector3 relativePos = xzTargetPos - transform.position;
            var rot = Quaternion.LookRotation(relativePos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);

            if (sheepController.CanGoForward())
            {
                distanceToTarget = (transform.position - target).sqrMagnitude;

                if (distanceToTarget > 0.5f * 0.5f)
                {
                    //rotates perpendicular to the ground
                    Vector3 forward = transform.TransformDirection(Vector3.forward);
                    sheepController.controller.SimpleMove(forward * Speed);
                }
                else
                {
                    bool wantEat = Random.Range(0, 1f) < 0.2f;

                    if (wantEat)
                    {
                        Eating();
                        //StartCoroutine(Eating(EatTime));
                    }

                    goingToTarget = false;
                }
            }
            else
            {
                goingToTarget = false;
            }
        }
    }

    void Eating()
    {
        sheepController.audioSource.pitch = Random.Range(0.9f, 1.1f);
        sheepController.audioSource.Play();
    }

    //temp eat animation
    //IEnumerator Eating(float time)
    //{
    //    eating = true;
    //    var clr = mesh.material.color;
    //    for (float ft = time; ft >= 0; ft -= 0.01f)
    //    {
    //        if (ft < 0.01f) ft = 0;

    //        mesh.material.color = new Color(ft, ft, ft);

    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    mesh.material.color = clr;
    //    eating = false;
    //}

    Vector3 GetRandomPointOnTheGroundInRange(float range)
    {
        Vector3 rndPoint = new Vector3(Random.Range(-range, range), 10, Random.Range(-range, range));

        if (Physics.Raycast(transform.position + rndPoint, Vector3.down, out var hitG, 100, sheepController.groundMask))
        {
            if (!Physics.SphereCast(transform.position + rndPoint, sheepController.DstBetweenSheeps, Vector3.down, out var hitO, 100f, sheepController.obstacleMask))
            {
                rndPoint = new Vector3(hitG.point.x, hitG.point.y + (transform.localScale.y / 2) + 0.01f, hitG.point.z);

                if (!Physics.Raycast(transform.position, rndPoint - transform.position, out var hitP, 100f, sheepController.obstacleMask))
                {
                    return rndPoint;
                }
            }
        }

        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);
    }
}
