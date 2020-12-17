using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float verticalSpeedMultiplier = 3.5f;
	[SerializeField] private float horizontalSpeedMultiplier = 3.5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

	private new CapsuleCollider collider = null;
	private bool isCrouching = false;
    private bool isSprinting = false;

	private void Start() 
	{
		this.collider = this.GetComponent<CapsuleCollider>();	
	}

	private void Update()
	{
        this.isSprinting = Input.GetKey(KeyCode.LeftShift);

		// Crouch Input
		if (Input.GetKeyDown(KeyCode.C) ||Input.GetKeyDown(KeyCode.LeftControl)
				|| Input.GetKeyUp(KeyCode.LeftControl))
		{
			this.Crouch(!this.isCrouching);
		}
	}	

	private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal") * this.horizontalSpeedMultiplier;
        float vertical = Input.GetAxis("Vertical") * this.verticalSpeedMultiplier;
		float sprintMulti = (this.isSprinting) ? this.sprintMultiplier : 1.0f;

		// Move/Translate the player on each axis.
        this.transform.Translate(Vector3.right * horizontal * sprintMulti * Time.fixedDeltaTime); 
        this.transform.Translate(Vector3.forward * vertical * sprintMulti * Time.fixedDeltaTime);
    }

	private void Crouch(bool shouldCrouch)
	{
		this.collider.center = (shouldCrouch) ? new Vector3(0.0f, 0.2f, 0.0f) : Vector3.zero;
		this.collider.height = (shouldCrouch) ? 1.6f : 2.0f;

		this.isCrouching = shouldCrouch;
	}
}
