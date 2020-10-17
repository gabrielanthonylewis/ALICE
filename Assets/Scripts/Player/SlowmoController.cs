using UnityEngine;
using UnityEngine.UI;

public class SlowmoController : MonoBehaviour
{
    [SerializeField] private RectTransform remainingTimeBar;
    [SerializeField] private RawImage screenOverlay;
    [SerializeField] private float maxTime = 7.0f; // in seconds
    [SerializeField] private float slowmoTimescale = 0.4f;

    public float remainingTime { get; private set; }
    private float barFillUnit; // to scale the fill uniformly no matter the maxTime
    private bool isSlomoActive = false;

    private void Awake()
    {
        this.remainingTime = this.maxTime;
        this.barFillUnit = this.remainingTimeBar.rect.width / this.maxTime;
        this.isSlomoActive = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            this.SetSlomoState(true);
        else if (Input.GetKeyUp(KeyCode.Space))
            this.SetSlomoState(false);

        if(this.isSlomoActive)
            this.ReduceRemainingTime();
    }

    private void SetSlomoState(bool shouldSlow)
    {
        this.isSlomoActive = (shouldSlow && this.remainingTime > 0.0f);
        Time.timeScale = (shouldSlow) ? this.slowmoTimescale : 1.0f;
        this.screenOverlay.enabled = shouldSlow;
    }

    private void ReduceRemainingTime()
    {
        // "1 / x" is to counter the slowed time
        float dt = (Time.deltaTime * (1.0f / Time.timeScale)); 
        this.SetRemainingTime(this.remainingTime - dt);
    }

    public void SetRemainingTime(float remainingTime)
    {
        this.remainingTime = remainingTime;

        if (this.remainingTime <= 0.0f)
        {
            this.remainingTime = 0.0f;
            this.SetSlomoState(false);
        }

        this.remainingTimeBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
            0.0f, this.remainingTime * this.barFillUnit);            
    }
}
