using UnityEngine;

public class Platform : MonoBehaviour 
{
	[SerializeField] private Transform pointA = null;
	[SerializeField] private Transform pointB = null;
	[SerializeField] private float duration = 1.0f;
	
	private Transform target = null;
	private float lerpT = 1.0f;
	private Vector3 fromPos;
	private Quaternion fromRot;

	private void Update()
	{
		if(this.target == null)
			return;

		// Lerp position and rotation.
		this.lerpT = Mathf.Min(this.lerpT + (Time.deltaTime / this.duration), 1.0f);
		this.transform.position = Vector3.Lerp(this.fromPos, this.target.position, this.lerpT);
		this.transform.rotation = Quaternion.Lerp(this.fromRot, this.target.rotation, this.lerpT);
	
		if(this.lerpT == 1.0f)
			this.OnReachedTarget();
	}

	protected virtual void OnReachedTarget() 
	{
		this.target = null;
	}

	// Upon activation go to the next target.
	public void Activate()
	{
		if(this.lerpT != 1.0f)
			return;
		
		this.target = (this.transform.position == this.pointB.position) ? this.pointA : this.pointB;
		
		this.fromPos = this.transform.position;
		this.fromRot = this.transform.rotation;
		this.lerpT = 0.0f;
	}

	// Take Rigidbodies with the platform.
	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<Rigidbody>())
			other.transform.SetParent(this.transform);
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<Rigidbody>())
			other.transform.SetParent(null);
	}
}
