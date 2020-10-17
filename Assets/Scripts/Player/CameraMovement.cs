﻿using UnityEngine;
using System.Collections;

// The CameraMovement script allows the recieving object (the camera is the intended object)
// to rotate depending on the mouse input. The X and Y axis can be independtly locked.
public class CameraMovement : MonoBehaviour
{
	[SerializeField] private bool LockX = false;
	[SerializeField] private bool LockY = false;
	[SerializeField] private float _VerticalSpeed = 2.5f;
	[SerializeField] private float _HorizontalSpeed = 2.5f;

    private float _VertLimitLow = 0.5f;
	private float _VertLimitHigh = -0.4f;

    private Transform _Transform = null;
	public Camera LayerCamera = null;

    void Awake()
    {
        _Transform = this.transform;
    }

    void Update()
    {
		// If paused (timeScale == 0), stop behaviour (return).
		if(Time.timeScale == 0)
			return;
		     
		// If X axis isn't locked then update the Vertical Rotation accordingly.
        if (!LockX)
            UpdateHorizontalRot();

		// If Y axis isn't locked then update the Vertical Rotation accordingly.
        if (!LockY)
			UpdateVerticalRot ();
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
		float mouseX = Input.GetAxis ("Mouse X") * _HorizontalSpeed;

		Screen.lockCursor = (mouseX != 0);

		// Get horizontal rotation freezing x and z axis.
        Quaternion horizontalRot = _Transform.rotation * Quaternion.Euler(new Vector3(0f, mouseX, 0f)) ;
        horizontalRot.x = 0f;
        horizontalRot.z = 0f;

        _Transform.rotation = horizontalRot;
    }

    private void UpdateVerticalRot()
    {
        float mouseY = Input.GetAxis("Mouse Y") * _VerticalSpeed;

		Screen.lockCursor = (mouseY != 0);

		// Get horizontal rotation freezing y and z axis.
        Quaternion verticalRot = Quaternion.Euler(new Vector3(-mouseY, 0f, 0f));
        verticalRot.x = Mathf.Clamp(verticalRot.x, Quaternion.Euler(-65f, 0, 0).x, Quaternion.Euler(65f, 0, 0).x);
        verticalRot.y = 0f;
        verticalRot.z = 0f;

		// Enforce upper and lower bound limits preventing the camera from rotating past these limits.
        if (_Transform.localRotation.x + verticalRot.x > _VertLimitLow || _Transform.localRotation.x + verticalRot.x < _VertLimitHigh)
            return;

        _Transform.rotation *= verticalRot;
    }

	void OnTriggerStay(Collider other)
	{
		//this.transform.position = Vector3.Lerp (this.transform.position, this.transform.position + this.transform.forward, Time.deltaTime);
		
	}


}
