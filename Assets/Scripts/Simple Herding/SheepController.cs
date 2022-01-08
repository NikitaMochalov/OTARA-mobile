using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Graze))]
[RequireComponent(typeof(Fright))]
public class SheepController : MonoBehaviour
{
    [Space]
    [Header("Raycast Layers")]
    public LayerMask groundMask;
    public LayerMask obstacleMask;
    public LayerMask sheepMask;

    [Space]
    [Header("Settings")]
    public float DstToObstacles = 2f;
    public float DstBetweenSheeps = 1f;
    public float ViewingAngle = 120;
    public int DensityOfView = 10;

    [Space]
    public CharacterController controller;

    [HideInInspector]
    public Vector3 dogPosition;

    [HideInInspector]
    public bool isScared;

    SheepsManager sheepsManager;
    [HideInInspector]
    public AudioSource audioSource;
    Graze graze;
    Fright fright;

    void Start()
    {
        sheepsManager = transform.parent.GetComponent<SheepsManager>();
        audioSource = GetComponent<AudioSource>();
        graze = GetComponent<Graze>();
        fright = GetComponent<Fright>();
        dogPosition = sheepsManager.Dog.position;
        fright.enabled = false;
        graze.enabled = true;

        #region start standing on the ground

        var ray = new Ray(transform.position + controller.center, -transform.up);

        if (Physics.Raycast(ray, out var hitInfo, 100f, groundMask))
        {
            if (Physics.SphereCast(transform.position, 3, -transform.up, out var hit, 100f, obstacleMask))
            {
                Destroy(gameObject);
            }
        }
        else Destroy(gameObject);
        #endregion
    }

    private void Update()
    {
        if (!isScared)
        {
            fright.enabled = false;
            graze.enabled = true;
        }

        if (isScared)
        {
            graze.enabled = false;
            fright.enabled = true;
        }
    }

    public void Scared()
    {
    }

    RaycastHit[] hitO;
    RaycastHit[] hitS;

    public void ObstaclesAvoidance()
    {
        var avoidanceDir = transform.forward.normalized;

        hitO = new RaycastHit[DensityOfView];
        hitS = new RaycastHit[DensityOfView];

        float angle = ViewingAngle;
        float angleDif = angle * 2 / DensityOfView;
        var raysOrigin = transform.position + controller.center;

        for (int i = 0; i < DensityOfView; i++)
        {
            var dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward * DstBetweenSheeps;
            dir.y = transform.forward.y;
            Debug.DrawRay(raysOrigin, dir, Color.blue);
            if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitO[i], DstToObstacles, obstacleMask))
            {
                avoidanceDir += hitO[i].normal * 50;
                break;
            }
            else if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitS[i], DstBetweenSheeps, sheepMask))
            {
                avoidanceDir += hitS[i].normal * 50;
                break;
            }
            angle = angle - angleDif;
        }
        var rot = Quaternion.LookRotation(avoidanceDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.5f);
    }

    public bool CanGoForward()
    {
        hitO = new RaycastHit[DensityOfView];
        hitS = new RaycastHit[DensityOfView];

        float angle = ViewingAngle;
        float angleDif = angle * 2 / DensityOfView;
        var raysOrigin = transform.position + controller.center;

        for (int i = 0; i < DensityOfView; i++)
        {
            var dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward * DstBetweenSheeps;
            dir.y = transform.forward.y;
            Debug.DrawRay(raysOrigin, dir, Color.blue);
            if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitO[i], DstToObstacles, obstacleMask))
            {
                return false;
            }
            //else if (Physics.SphereCast(raysOrigin, transform.localScale.x, dir, out hitS[i], DstBetweenSheeps, sheepMask))
            //{
            //    return false;
            //}
            angle = angle - angleDif;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hitO != null)
        {
            //draw point of collision with obstacles
            for (int i = 0; i < hitO.Length; i++)
            {
                if (hitO[i].point != Vector3.zero)
                    Gizmos.DrawSphere(hitO[i].point, 0.2f);
            }
        }
        if (hitO != null)
        {
            //draw point of collision with another sheep
            for (int i = 0; i < hitS.Length; i++)
            {
                if (hitS[i].point != Vector3.zero)
                    Gizmos.DrawSphere(hitS[i].point, 0.1f);
            }
        }

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position + controller.center, DstBetweenSheeps);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(target, 0.1f);
    }
}