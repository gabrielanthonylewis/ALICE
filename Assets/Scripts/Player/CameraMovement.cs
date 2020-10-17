using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField] private bool LockX = false;
	[SerializeField] private bool LockY = false;
	[SerializeField] private float verticalSpeed = 2.5f;
	[SerializeField] private float horizontalSpeed = 2.5f;
    [SerializeField] float verticalLowerLimit = 0.5f;
	[SerializeField] float verticalUpperLimit = -0.4f;

    private void Update()
    {
		bool isPaused = (Time.timeScale == 0);
		if(isPaused)
			return;
		     
        if (!this.LockX)
            this.UpdateHorizontalRot();
        if (!this.LockY)
			this.UpdateVerticalRot ();
    }

	 /*
    private bool tiltRight = false, tiltLeft = false;

    void Update ()
    {
		// Tilt Right OR back to the normal state depending on current tilt state.
		if (Input.GetKeyDown (KeyCode.E)) 
		{
			tiltRight = !tiltRight;

			// Play backwards/forwards depending on the current tilt state.
			if (tiltRight == true)
				this.GetAnimation () ["tiltRight"].speed = 1;
			else
				this.GetAnimation () ["tiltRight"].speed = -1;

			this.GetAnimation ().Play ("tiltRight");
		}

		// Tilt Left OR back to the normal state depending on current tilt state.
		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			tiltLeft = !tiltLeft;

			// Play backwards/forwards depending on the current tilt state.
			if (tiltLeft == true)
				this.GetAnimation () ["tiltLeft"].speed = 1;
			else
				this.GetAnimation () ["tiltLeft"].speed = -1;

			this.GetAnimation ().Play ("tiltLeft");
		}
    }
    */

    private void UpdateHorizontalRot()
    {
		float mouseX = Input.GetAxisRaw("Mouse X") * this.horizontalSpeed;
		this.transform.Rotate(Vector3.up, mouseX, Space.Self);
    }

    private void UpdateVerticalRot()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y") * this.verticalSpeed;

        Quaternion verticalRot = Quaternion.Euler(new Vector3(-mouseY, 0f, 0f));
        verticalRot.x = Mathf.Clamp(verticalRot.x, Quaternion.Euler(-65f, 0, 0).x, Quaternion.Euler(65f, 0, 0).x);

		// Enforce upper and lower bound limits preventing the camera from rotating past these limits.
        if (this.transform.localRotation.x + verticalRot.x > verticalLowerLimit || this.transform.localRotation.x + verticalRot.x < verticalUpperLimit)
            return;

        this.transform.rotation *= verticalRot;
    }

	void OnTriggerStay(Collider other)
	{
		//this.transform.position = Vector3.Lerp (this.transform.position, this.transform.position + this.transform.forward, Time.deltaTime);
		
	}


}
