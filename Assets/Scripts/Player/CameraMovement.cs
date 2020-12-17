using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField] private bool lockXAxis = false;
	[SerializeField] private bool lockYAxis = false;
	[SerializeField] private float verticalSpeed = 2.5f;
	[SerializeField] private float horizontalSpeed = 2.5f;
    [SerializeField] float verticalLowerLimit = 0.5f;
	[SerializeField] float verticalUpperLimit = -0.4f;

    private void Update()
    {
		bool isPaused = (Time.timeScale == 0);
		if(isPaused)
			return;
		     
        if (!this.lockXAxis)
            this.UpdateHorizontalRot();
        if (!this.lockYAxis)
			this.UpdateVerticalRot ();
    }

    private void UpdateHorizontalRot()
    {
		float mouseX = Input.GetAxisRaw("Mouse X") * this.horizontalSpeed;
		this.transform.Rotate(Vector3.up, mouseX, Space.Self);
    }

    private void UpdateVerticalRot()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y") * this.verticalSpeed;

        Quaternion verticalRot = Quaternion.Euler(new Vector3(-mouseY, 0f, 0f));

		// Enforce upper and lower bound limits preventing the camera from rotating past these limits.
        if (this.transform.localRotation.x + verticalRot.x > verticalLowerLimit)
			verticalRot.x = verticalLowerLimit - this.transform.localRotation.x;
		if(this.transform.localRotation.x + verticalRot.x < verticalUpperLimit)
			verticalRot.x = verticalUpperLimit - this.transform.localRotation.x;

        this.transform.rotation *= verticalRot;
    }

}
