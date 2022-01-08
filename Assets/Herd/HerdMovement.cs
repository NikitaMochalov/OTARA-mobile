using UnityEngine;

public class HerdMovement : MonoBehaviour
{

	public struct Threat
	{
		public Vector3 position;
		public Transform threatTransform;
		public bool movingThreat;
		public float timeLeft;
		public float originalTime;
		public float strength;
		public float maxRange;

		public Threat(Vector3 _position, float _strength, float _timeLeft, float _maxRange)
		{
			// this is called for static threats
			position = _position;
			timeLeft = _timeLeft;
			originalTime = _timeLeft;
			strength = _strength;
			maxRange = _maxRange;

			movingThreat = false;
			// this doesn't get used for a static threat
			threatTransform = null;
		}

		public Threat(Transform _transform, float _strength, float _timeLeft, float _maxRange)
		{
			// this is called for moving threats
			threatTransform = _transform;
			timeLeft = _timeLeft;
			originalTime = _timeLeft;
			strength = _strength;
			maxRange = _maxRange;

			movingThreat = true;
			// this doesn't get used for a moving threat
			position = Vector3.zero;
		}
	}

	HerdManager herdManager;

	public Transform target;
	public float targetWeight;
	public float targetRange;

	public Transform danger;
	public float dangerWeight;

	Threat[] allThreats;
	int numThreats;
	int maxThreats = 5;

	public Vector3 moveDirection;
	public float moveSpeed = 0.1f;

	[SerializeField]
	Vector3 alignment = Vector3.zero;
	public float alignmentWeight;
	[SerializeField]
	Vector3 cohesion = Vector3.zero;
	public float cohesionWeight;
	[SerializeField]
	Vector3 separation = Vector3.zero;
	public float separationWeight;
	[SerializeField]
	Vector3 threat = Vector3.zero;
	[SerializeField]
	Vector3 targetDir = Vector3.zero;

	/*
	Vector3 lastAlignment;
	Vector3 lastCohesion;
	Vector3 lastSeparation;
	*/

	void Start()
	{
		alignment = Vector3.zero;
		cohesion = Vector3.zero;
		separation = Vector3.zero;

		threat = Vector3.zero;
		numThreats = 0;
		allThreats = new Threat[maxThreats];

		herdManager = FindObjectOfType<HerdManager>();
		herdManager.AddToHerd(this);
		moveDirection = Vector3.zero;

		/*
		lastAlignment = alignment;
		lastCohesion = cohesion;
		lastSeparation = separation;
		*/

		RandomStats();
	}

	void RandomStats()
	{
		// numbers are just what looks about right
		alignmentWeight = Random.Range(0.5f, 3f);
		cohesionWeight = Random.Range(0.25f, 2f);
		// if separation weight was lower than 5, they tend to run into one another
		//separationWeight = Random.Range ();
		moveSpeed = Random.Range(0.2f, 0.4f);
		dangerWeight = Random.Range(0.85f, 1.1f);
	}

	public void Threaten(Vector3 threatPos, float threatStrength, float threatDuration, float threatMaxRange, bool fading)
	{
		// add new threat to list of threats
		if (numThreats >= maxThreats)
		{
			return;
			// no new threats can be handled
		}
		allThreats[numThreats] = new Threat(threatPos, threatStrength, threatDuration, threatMaxRange);
		numThreats++;
	}

	/// <summary>
	/// For static threats.
	/// Such as a campfire.
	/// </summary>
	/// <param name="threatPos">Threat position.</param>
	/// <param name="threatStrength">Threat strength.</param>
	/// <param name="threatDuration">Threat duration.</param>
	/// <param name="threatMaxRange">Threat max range.</param>
	public void Threaten(Vector3 threatPos, float threatStrength, float threatDuration, float threatMaxRange)
	{
		// default is fading threat
		Threaten(threatPos, threatStrength, threatDuration, threatMaxRange, true);
	}

	/// <summary>
	/// For threats that move. 
	/// Such as if a shepherd yells, the sheep will be threatened by the shepherd, not the position they were in when they yelled.
	/// </summary>
	/// <param name="threatTrans">Threat trans.</param>
	/// <param name="threatStrength">Threat strength.</param>
	/// <param name="threatDuration">Threat duration.</param>
	/// <param name="threatMaxRange">Threat max range.</param>
	public void MoveThreaten(Transform threatTrans, float threatStrength, float threatDuration, float threatMaxRange)
	{
		// add new threat to list of threats
		if (numThreats > maxThreats)
		{
			return;
			// no new threats can be handled
		}
		allThreats[numThreats] = new Threat(threatTrans, threatStrength, threatDuration, threatMaxRange);
		numThreats++;
	}

	void OnDrawGizmosSelected()
	{
		// multiply all by 10 so they are easier to see in game view
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, threat * 10);

		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(transform.position, alignment * 10);

		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, cohesion * 10);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, separation * 10);

		Gizmos.color = Color.white;
		Gizmos.DrawRay(transform.position, targetDir * 10);
		//Gizmos.color = Color.gray;
		//Gizmos.DrawRay (transform.position, moveDirection * 10);
	}

	void FixedUpdate()
	{
		alignment = herdManager.ComputeAlignment(this) * alignmentWeight;
		cohesion = herdManager.ComputeCohesion(this) * cohesionWeight;
		separation = herdManager.ComputeSeparation(this) * separationWeight;

		//TODO this should be governed my max turn radius instead
		// the sheep shouldn't be albe to stop instantly, turn on a dime or find the next nearest food source so immedietely
		// hopefully this smooths things out slightly
		// without this, the separate herds pull apart easier
		// seems to be less stuttering around where there's the tug-of-war
		/*
		alignment = ((alignment + lastAlignment) / 2f) ;
		cohesion = ((cohesion + lastCohesion) / 2f) ;
		separation = ((separation + lastSeparation) / 2f) ;

		lastAlignment = alignment;
		lastSeparation = separation;
		lastCohesion = cohesion;
		*/
		moveDirection = alignment + cohesion + separation;

		// move towards a target position
		// the closer you are, the harder the pull in that direction
		// max so that the herd doesn't move away from the target when further away than max range
		// 0.5f is so that the herd doesn't get totally lost. They will alway pull a little bit towards the target
		targetDir = (target.position - transform.position).normalized * targetWeight * Mathf.Max(0.5f, (targetRange - Vector3.Distance(target.position, transform.position))) / targetRange;
		// could be
		// max force of 3 (could be a variable)
		// instead of using target weight
		// targetDir = (target.position-transform.position).normalized * targetWeight * Mathf.Max(3f, Mathf.Max(0.5f, (targetRange - Vector3.Distance(target.position, transform.position))));
		moveDirection += targetDir;

		// move away from the danger
		// closer you are, stronger push away is
		// set threat so that you can see it in game as a ray
		// this is for the threat as an object in the world
		threat = (transform.position - danger.position).normalized * dangerWeight * Mathf.Max(0.01f, (10f - Vector3.Distance(transform.position, danger.position)));
		moveDirection += threat;

		// this is for the threats that the player makes
		if (numThreats > 0)
		{
			for (int i = 0; i < numThreats; i++)
			{
				Vector3 threatPosition;
				if (allThreats[i].movingThreat)
				{
					threatPosition = allThreats[i].threatTransform.position;
				}
				else
				{
					threatPosition = allThreats[i].position;
				}

				// (allThreats[i].timeLeft / allThreats[i].originalTime) makes the threat fade over time.
				moveDirection += (transform.position - threatPosition).normalized * allThreats[i].strength * (allThreats[i].timeLeft / allThreats[i].originalTime) * (Mathf.Max(0.0f, allThreats[i].maxRange - Vector3.Distance(transform.position, threatPosition)));

				allThreats[i].timeLeft -= Time.deltaTime;
				// this maybe could be its own function
				if (allThreats[i].timeLeft < 0f)
				{
					// threat over; remove from array
					for (int j = i; j < numThreats - 1; j++)
					{
						allThreats[j] = allThreats[j + 1];
					}
					// removed a threat so number of threats goes down
					numThreats--;
					// this threat is gone, but a new one might be in its place so do this index of the array again
					i--;
				}
			}
		}

		// doesn't get as messsed up by collisions
		// this function doesn't worry about if moveDirection is magnitude 1 or magnitude 50, it still moves the same speed
		// if moveDirection magnitude is < 1, then it will move slowly. This happens when too far away from neighbours and its target.
		//transform.position = Vector3.MoveTowards (transform.position, transform.position + moveDirection, moveSpeed);
		GetComponent<CharacterController>().SimpleMove(moveDirection * moveSpeed * 10);
	}
}