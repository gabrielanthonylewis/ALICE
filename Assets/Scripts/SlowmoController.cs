using UnityEngine;
using UnityEngine.UI;

public class SlowmoController : MonoBehaviour
{
    [SerializeField] private RectTransform barFill;
    [SerializeField] private RawImage screenFill;
    [SerializeField] private float maxTime = 7.0f; // in seconds
    [SerializeField] private float slowTimeScale = 0.4f;

    private float remainingTime;
    private float fixedDeltaTime;
    private float barFillUnit; // to scale the fill uniformly no matter the maxTime

    private void Awake()
    {
        this.remainingTime = this.maxTime;
        this.barFillUnit = this.barFill.rect.width / this.maxTime;
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (this.remainingTime <= 0.0f)
            return;

        if (Input.GetKeyUp(KeyCode.Space))
            this.StopSlowmo();
        else if (Input.GetKeyDown(KeyCode.Space))
            this.StartSlowmo();
        else if (Input.GetKey(KeyCode.Space))
            this.ReduceRemainingTime();
    }

    private void StartSlowmo()
    {
        Time.timeScale = this.slowTimeScale; // slow time
        Time.fixedDeltaTime = this.fixedDeltaTime * this.slowTimeScale; // slow physics
        this.screenFill.enabled = true;
    }

    private void ReduceRemainingTime()
    {
        float newTime = this.remainingTime - Time.deltaTime * (1.0f / Time.timeScale); // "1 / x" is to counter the slowed time
        this.SetRemainingTime(newTime);
    }

    private void StopSlowmo()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = this.fixedDeltaTime;
        this.screenFill.enabled = false;
    }

    public void SetRemainingTime(float remainingTime)
    {
        this.remainingTime = remainingTime;

        this.barFill.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0.0f, this.remainingTime * this.barFillUnit);

        if (this.remainingTime <= 0.0f)
            this.StopSlowmo();
    }

    public float GetRemainingTime()
    {
        return this.remainingTime;
    }
}
