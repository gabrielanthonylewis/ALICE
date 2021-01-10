using UnityEngine;
using UnityEngine.Events;

public class RotationPuzzlePiece : MonoBehaviour, IInteractable
{
	[SerializeField] private WheelSegment segmentType;
	[SerializeField] private GameObject[] inputs;
	[SerializeField] private WheelInput currentInput;

	[HideInInspector] public UnityEvent<WheelSegment, WheelInput> onRotate 
		= new UnityEvent<WheelSegment, WheelInput>();

	private int inputCount;
	private float rotationStep;

	private void Awake()
	{
		this.inputCount = System.Enum.GetValues(typeof(WheelInput)).Length;
		this.rotationStep = (this.inputCount > 0) ? 360.0f / this.inputCount : 0.0f;
	}

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		if(isDownOnce)
			this.Rotate();
	}

	private void Rotate()
	{
		this.transform.Rotate(0.0f, 0.0f, -this.rotationStep, Space.World);

		this.currentInput = (WheelInput)Mathf.Repeat((int)this.currentInput - 1, this.inputCount);

		this.onRotate.Invoke(this.segmentType, this.currentInput);
	}

	public GameObject[] getInputs()
	{
		return this.inputs;
	}
}
