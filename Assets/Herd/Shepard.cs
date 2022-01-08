using UnityEngine;

public class Shepard : MonoBehaviour
{
	public HerdManager herdManager;
	public float threatStrenght = 1;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			// swing staff to get herd to move away from you
			herdManager.ThreatenHerd(transform.position, threatStrenght, 2f, 10f);
		}
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			// whistle to get herd to follow you
			// pass in the transform to get the threat to follow you
			// if strength is larger than separation weight on the herd, they will run into you and push you around
			herdManager.MovingThreat(transform, -0.1f, 4f, 35f);
		}
	}
}