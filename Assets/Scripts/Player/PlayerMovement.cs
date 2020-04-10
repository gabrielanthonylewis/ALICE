using UnityEngine;

// The PlayerMovement script moves the object depending on the player inputs, also providing crouch and prone functionality.
public class PlayerMovement : MonoBehaviour
{
	// Vertical Speed multiplier
    [SerializeField] private float _VerticalSpeed = 6f;
    
	// Horizontal Speed multiplier
	[SerializeField] private float _HorizontalSpeed = 6f;

    [SerializeField] private float _sprintMultiplier = 2.0f;

	// Is the Player Crouched?
	private bool isCrouching = false;

    private bool _isSprinting = false;

	void Update()
	{
        // Sprint
        _isSprinting = Input.GetKey(KeyCode.LeftShift);

		// Crouch/Stand back up depending on the current stance.
		if (Input.GetKeyDown(KeyCode.C))
		{
			if(isCrouching)
			{
				// return to original size
				this.GetComponent<CapsuleCollider>().center = new Vector3(0f,0f,0f);
				this.GetComponent<CapsuleCollider>().height = 2f;
				isCrouching = false;
				return;
			}

			// New size (Crouch)
			// NOTE: Crouching works by simply changing the size of the collider and moving it up 
			// letting the player fall beneath the floor more so than before.
			this.GetComponent<CapsuleCollider>().height = 1.6f;
			this.GetComponent<CapsuleCollider>().center = new Vector3(0f,0.2f,0f);
			isCrouching = true;
		}		
	}

    void FixedUpdate()
    {
		// Record horizontal and vertical movement multiplying each by their corresponding multiplier.
        float horizontal = Input.GetAxis("Horizontal") * _HorizontalSpeed;
        float vertical = Input.GetAxis("Vertical") * _VerticalSpeed;

        // Sprint
        float sprintMulti = 1.0f;
        if (_isSprinting)
            sprintMulti = _sprintMultiplier;

		// Move/Translate the player on each axis.
        transform.Translate(new Vector3(horizontal, 0f, 0f) * Time.fixedDeltaTime * sprintMulti); 
        transform.Translate(new Vector3(0, 0f, vertical) * Time.fixedDeltaTime * sprintMulti);
    }
	
}
