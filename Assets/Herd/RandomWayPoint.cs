using UnityEngine;

public class RandomWayPoint : MonoBehaviour
{
    public float maxRandom = 25;

    private void Start()
    {
        transform.Translate(0, 100, 0);
        if(Physics.Raycast(new Ray(transform.position, -transform.up), out var hit,  200))
            {
            transform.position = hit.point;
        }
    }

    // this is mostly just for testing to see if the herding works
    void OnTriggerEnter(Collider col)
    {
        transform.position = new Vector3(Random.Range(-maxRandom, maxRandom), 100, Random.Range(-maxRandom, maxRandom));
        if (Physics.Raycast(new Ray(transform.position, -transform.up), out var hit, 200))
        {
            transform.position = hit.point;
        }
	}
}