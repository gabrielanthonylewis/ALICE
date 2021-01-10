﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Destructable : MonoBehaviour 
{
	[SerializeField] protected float maxHealth = 1.0f;
	[SerializeField] protected float currentHealth = 1.0f;
	[SerializeField] private GameObject onDestroyedFragments = null;
	[SerializeField] private GameObject[] dropList = new GameObject[0];
	[SerializeField] private AnimationClip deathAnimation = null;
	[SerializeField] private AnimationClip hitAnimation = null;
	[SerializeField] private Text worldHealth = null;
	[SerializeField] private UnityEvent onDeath = new UnityEvent();
	[SerializeField] private int pickupLayer = 8;

	private new Animation animation = null;

	private void Awake()
	{
		this.animation = this.GetComponent<Animation>();

		this.RefreshUI();
	}

    public void SetHealth(float health)
    {
        this.currentHealth = Mathf.Clamp(health, 0.0f, this.maxHealth);

		if (this.currentHealth <= 0.0f)
			this.StartCoroutine(this.OnDeathRoutine());

		this.RefreshUI();
	}

    public float GetHealth()
    {
        return this.currentHealth;
    }

	public bool ManipulateHealth(float val)
	{
		if((this.currentHealth == this.maxHealth && val >= 0.0f) ||
			(this.currentHealth == 0.0f && val <= 0.0f))
		{
			return false;
		}

		this.SetHealth(this.currentHealth + val);

		AnimationUtils.PlayAnimationClip(this.animation, this.hitAnimation);

		return true;
	}

	protected virtual void RefreshUI()
	{
		if(this.worldHealth != null)
			this.worldHealth.text = (this.currentHealth + "/" + this.maxHealth);
	}


	private IEnumerator OnDeathRoutine()
	{
		if(this.animation != null && this.deathAnimation != null)
		{
			AnimationUtils.PlayAnimationClip(this.animation, this.deathAnimation);
			
			yield return new WaitForSeconds(this.deathAnimation.length);
		}

		this.OnDeath();
	}

	protected virtual void OnDeath()
	{
		if(this.onDestroyedFragments != null) 
		{
			GameObject.Instantiate(this.onDestroyedFragments,
				this.transform.position, this.transform.rotation);
		}	

		// Drop all objects in the Droplist and add physics.
		foreach(GameObject drop in this.dropList)
		{
			drop.transform.SetParent(null);

			Rigidbody droppedRigidbody = drop.GetComponent<Rigidbody>();
			if(droppedRigidbody == null)
				droppedRigidbody = drop.AddComponent<Rigidbody>();
			droppedRigidbody.useGravity = true;
			droppedRigidbody.isKinematic = false;
			
			drop.layer = this.pickupLayer;
		}

		this.onDeath.Invoke();

		GameObject.Destroy(this.gameObject);
	}
}
