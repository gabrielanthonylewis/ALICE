using UnityEngine;
using System.Collections;

public class SequenceController : PuzzleBase, IInteractable 
{
	[SerializeField] private SequenceButton[] sequenceButtons;
	[SerializeField] private Color completedColour;
	[SerializeField] private int sequenceLength;

	private bool isCoroutineRunning = false;
	private SequenceButton[] sequence = new SequenceButton[0];
	private int currAttemptIdx = 0;

	private void Start()
	{
		this.GenerateNewSequence();
	}

	private void GenerateNewSequence()
	{
		this.sequence = new SequenceButton[this.sequenceLength];
		this.currAttemptIdx = 0;

		for(int i = 0; i < this.sequence.Length; i++)
			this.sequence[i] = this.sequenceButtons[Random.Range(0, this.sequenceButtons.Length)];
	}

	public void OnInteract(GameObject interactor, bool isDownOnce)
	{
		if(this.isComplete)
			return;

		this.TryPlaySequence();
	}

	private void TryPlaySequence()
	{
		if(this.isCoroutineRunning)
			return;

		this.StartCoroutine(this.PlaySequenceRou());
	}

	private IEnumerator PlaySequenceRou()
	{
		this.isCoroutineRunning = true;

		yield return new WaitForSeconds(1.0f);

		// Flash the buttons in order of the sequence.
		foreach(SequenceButton sequenceButton in this.sequence)
		{
			sequenceButton.Flash();

			yield return new WaitForSeconds(0.8f);
		}

		this.isCoroutineRunning = false;
	}

	public bool CanAddAttempt()
	{
		return (!this.isCoroutineRunning && !this.isComplete);
	}

	public void AddButtonAttempt(SequenceButton button)
	{
		bool isCorrectAttempt = (button == this.sequence[this.currAttemptIdx]);
		if(isCorrectAttempt)
		{
			// if the attempt has been finished...
			if(this.currAttemptIdx == this.sequenceLength - 1)
			{		
				this.GetComponent<MeshRenderer>().material.color = this.completedColour;
				this.OnComplete();
			}
			
			this.currAttemptIdx++;
		}
		else
		{
			// Flash both buttons to symbolise an incorrect attempt.
			foreach(SequenceButton sequenceButton in this.sequenceButtons)
				sequenceButton.Flash();

			// Generate a new sequence and then play to the user.
			this.GenerateNewSequence();
			this.TryPlaySequence();
		}
	}
}
