using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    [SerializeField] private Text timeText = null;
	[SerializeField] private float duration = 107.0f;
	[SerializeField] private UnityEvent onFinished = new UnityEvent();

	private float remainingTime = 0.0f;

    private void OnEnable()
	{
		this.remainingTime = this.duration;
	}

	private void Update()
	{
		if(this.remainingTime == 0.0f)
			return;

		this.remainingTime = Mathf.Max(0.0f, this.remainingTime - Time.deltaTime);

		// display 00:00 (Minutes:Seconds)
		float minutes = Mathf.FloorToInt(this.remainingTime / 60.0f);
		float seconds = Mathf.FloorToInt(this.remainingTime % 60.0f);
		this.timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

		if(this.remainingTime == 0.0f)
			this.onFinished.Invoke();
	}
}
