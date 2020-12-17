using UnityEngine;

public class PlayerHealth : Destructable
{
	[SerializeField] private RectTransform healthBar = null;
	[SerializeField] private GameObject deathCam = null;
    [SerializeField] private float slomoTimeScale = 0.4f;
    
	// A scaled value (representing 1 health) for use of scaling the bar.
	private float healthBarUnit = 0.0f;

    private void Awake()
    {
        this.healthBarUnit = this.healthBar.rect.width / this.maxHealth;  
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        const float inset = 18.0f;
        this.healthBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
            inset, this.currentHealth * this.healthBarUnit);
    }

    protected override void OnDeath()
    {
		if(this.deathCam != null) 
		{
            this.deathCam.transform.SetParent(null);
			this.deathCam.transform.GetChild(0).transform.position = this.transform.position;
			this.deathCam.SetActive (true);

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;

			Time.timeScale = this.slomoTimeScale;
		}
        
        base.OnDeath();
    }
}
