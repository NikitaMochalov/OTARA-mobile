using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepsManager : MonoBehaviour
{
    public Transform Dog;

    [HideInInspector]
    public List<Transform> Sheeps;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Sheeps.Add(transform.GetChild(i));
        }
        GameManager.Instance.SetSheepsCount(Sheeps.Count);
    }
}
