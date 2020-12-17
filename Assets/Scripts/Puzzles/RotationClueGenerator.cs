using UnityEngine;

public class RotationClueGenerator : MonoBehaviour 
{
	// Which segment the clue should copy.
	[SerializeField] private WheelSegment wheelSegment = WheelSegment.Inner;
	[SerializeField] private RotationPuzzleParent rotationPuzzleParent = null;

	private void Start() 
	{
		if(this.rotationPuzzleParent == null)
			return;

		// Change colour to match the correct sequence value.
		this.transform.GetComponent<MeshRenderer>().material
			= this.rotationPuzzleParent.GetSequenceMaterial((int)this.wheelSegment);
	}

}
