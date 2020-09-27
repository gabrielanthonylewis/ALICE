using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// The AI Weapon Controller script manages the weapon specfic behaviour automatiaclly for the AI.
// It deal with Reloading and firing as well as Spotting the targets through multiple ray casts.
public class AIWeaponController : MonoBehaviour 
{
	/*
        // TODO: put in AIWeaponController?
        // (Optional) AI ammo count (doesn't have a seperate inventory).
        [SerializeField] private int AiAmmo = 300;
        // is AI? (decided automatically).
        private bool isAI = false;

        // TODO: controller
        // Dependent on whether the Reload Coroutine is being run.
        private bool reloadRou = false;

        void Start()
        {

            // Automatically decide if weapon is owned by an AI.
            if (this.transform.parent == null)
                isAI = false;
            else if (this.transform.parent.GetComponent<AIWeaponController>())
                isAI = true;
            else
                isAI = false;
               
            // Manipulate ammo (because we initially load the clip).
            if (isAI)
                AIManipulateAmmo(-magSize);
            else
                Inventory.instance.ManipulateAmmo(gunType, -magSize);
        }

        public bool Reload()
        {
            // Play Reload animtion.
            if (isAI)
            {

                if (!reloadRou)
                    StartCoroutine("ReloadRou");

                return true;
            }

            return true;
        }

        IEnumerator ReloadRou()
        {
            reloadRou = true;

            // Deactivate muzzle flash.
            muzzleFlashGO.SetActive(false);

            // Play Reload _Animation and wait for it to be complete.
            _Animation.Play("reload");
            yield return new WaitForSeconds(_Animation["reload"].length);

            // Update clip and AI Ammo (taking into account the case where AI ammo cannot fill the clip).
            if (AiAmmo < magSize)
            {
                remainingAmmo = AiAmmo;
                AiAmmo = 0;
            }
            else
            {
                remainingAmmo = magSize;
                AiAmmo -= magSize;
            }

            reloadRou = false;
        }

        public void AIManipulateAmmo(int value)
        {
            AiAmmo += value;
            if (AiAmmo < 0)
                AiAmmo = 0;
        }
        */

    /*// Reference to the current weapon being used.
	[SerializeField] private Weapon currentWeapon = null;

	// The ParticleSystem to be instantiated upon hitting an object.
	[SerializeField] private ParticleSystem ObjectHitParticle;

	// Reference to the Player/Target
	[SerializeField] private GameObject Player = null;

	// There could potentially be more than one target (store in this variable)
	[SerializeField] private GameObject[] targets;

	// Tag of the Target (so that AI could potentially fight other AI/"Freindlys")
	[SerializeField] private string TargetTag = "Player";

	// How far the AI can see (units/meters)
	[SerializeField] private float _SightRange = 25f;

	[SerializeField] private float _RandomVectorRange = 0.12f;

	// Current index of the target in focus (corresponds with the "targets" array).
	private	int targetIndex = -1;

	// If true then the behaviour will be halted at a point. Used when AI is reacting (waits a moment of time). 
	private bool waitFor = false;

	// Prevents behaviour from executing if true.
	private bool stop = false;
	
	void Start () 
	{

		// If a Player target hasn't been set then find one.
		if(Player == null)
			Player = GameObject.FindGameObjectWithTag("Player");

		// Find all Gameobjects with the corresponding tag.
		targets = GameObject.FindGameObjectsWithTag(TargetTag);
			
		// Find the index of the current player/target in the array.
		for(int i = 0; i < targets.Length; i++)
		{
			if(targets[i] == Player)
				targetIndex = i;
		}

	}

	void Update ()
	{
		// if stop == true, halt behaviour.
		if(stop)
			return;

		// If there isn't a current target then stop firing.
		if (targetIndex < 0 || targetIndex > targets.Length || targets[targetIndex] == null)
		{
			// If there are no targets, try find one (or more) again.
			if(targets.Length == 0)
				targets = GameObject.FindGameObjectsWithTag(TargetTag);

			// Hide muzzle flash.
			currentWeapon.GetMuzzleFlashGO().SetActive (false);
		
			// Randomly pick a new target from the possible targets. (done 50 times in the case the target is null)
			for(int i = 0; i < 50; i++)
			{
				if(targets.Length == 0)
					break;

				targetIndex = Random.Range(0, targets.Length);

				// If a target is found, check to see if the AI (self) can see the target.
				if(targets[targetIndex] != null)
				{
					RaycastHit hit;
					if (Physics.Raycast (this.transform.position, targets[targetIndex].transform.position - this.transform.position, out hit, _SightRange))
					{
						if(hit.transform.tag == TargetTag)
							break;
					}
				}
			}

			// If no target is found, stop behaviour.
			if(targets.Length > 0 && targets[targetIndex] == null)
				stop = true;

			// The target is not spotted by default.
			SpottedPlayer = false;
		
			return;
		}

		// "Look" (aim gun) towards the player.
		this.transform.LookAt (targets[targetIndex].transform.position, transform.up);

		// If waitFor is true (in the case of reaction time), hault behaviour.
		if(waitFor)
			return;
	}

	// Wait a random amount of time before reacting.
	IEnumerator Reaction()
	{
		waitFor = true;

		float randomTime = Random.Range(0f,1.5f);
		yield return new WaitForSeconds(randomTime);

		waitFor = false;
	}
	
	public GameObject GetTarget()
	{
		if (targetIndex < 0 || targetIndex > targets.Length)
			return null;

		return targets[targetIndex];
	}

    */

}
