using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Vector3 cloudsHandlerPos;
    private CloudsManager cloudsManager;
    private float speed;
    private float limitValue;

    void Start()
    {
        cloudsHandlerPos = transform.parent.transform.position;
        cloudsManager = transform.parent.GetComponent<CloudsManager>();
        limitValue = cloudsManager.limitValue;
        speed = cloudsManager.generalSpeed;
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z > cloudsHandlerPos.z + limitValue)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, cloudsHandlerPos.z - limitValue);
        }
    }
}
