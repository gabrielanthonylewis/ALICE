using UnityEngine;

public enum WheelSegment
{
	Inner = 0,
	Middle = 1,
	Outer = 2
};

public enum WheelInput
{
	North = 0,
	East = 1,
	South = 2,
	West = 3
}

public class RotationPuzzleParent : PuzzleBase 
{
	[SerializeField] private RotationPuzzlePiece[] wheelPieces;
	[SerializeField] private Animation openDoorAnim;

	private WheelInput[] sequence;
	private WheelInput[] attempt;

	private void Awake() 
	{
		this.sequence = new WheelInput[this.wheelPieces.Length];
		this.attempt = new WheelInput[this.wheelPieces.Length];

		foreach(RotationPuzzlePiece piece in this.wheelPieces)
			piece.onRotate.AddListener(this.OnRotate);

		this.GenerateSequence();
	}
	
	private void GenerateSequence()
	{
		for(int i = 0; i < this.sequence.Length; i++)
		{
			/* Removes the possibility of the sequence being correct already by
			 * avoiding the use of the first value on the first wheel. */
			int minValue = 0;
			if(i == 0)
				minValue = 1;

			this.sequence[i] = (WheelInput)Random.Range(minValue,
				System.Enum.GetNames(typeof(WheelInput)).Length);
		}
	}

	private void TryComplete()
	{
		// Compare the sequence and the attempt.
		for(int i = 0; i < this.sequence.Length; i++)
		{
			if(this.attempt[i] != this.sequence[i])
				return;
		}

		this.OnComplete();
	}

    protected override void OnComplete()
    {
        base.OnComplete();

		foreach(RotationPuzzlePiece piece in this.wheelPieces)
			piece.gameObject.layer = 0;

		if(this.openDoorAnim != null)
			this.openDoorAnim.Play();
    }

	private void OnRotate(WheelSegment piece, WheelInput input)
	{
		this.attempt[(int)piece] = input;
		this.TryComplete();
	}

	public Material GetSequenceMaterial(int index)
	{
		GameObject wheelInput = this.wheelPieces[index].getInputs()[(int)this.sequence[index]];
		return wheelInput.GetComponent<MeshRenderer>().material;
	}

}
