using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private AudioClip actionCompleteSound = null;
    [SerializeField] private GameObject particles = null;
    [SerializeField] private string affectedObjectTag;
    private AudioSource audioSource = null;

    protected virtual bool AffectObject(Transform target) { return false; }

    private void Start()
    {
        this.audioSource = Camera.main.GetComponent<AudioSource>();
    }

    public bool TryAffectObject(Transform target)
    { 
        if (target == null || target.tag != this.affectedObjectTag)
            return false;

        return AffectObject(target);
    }

    public void SetActive(bool active)
    {
        this.particles.SetActive(active);
    }

    protected virtual void OnActionComplete(Transform target = null)
    {
        if(audioSource == null)
            this.audioSource = Camera.main.GetComponent<AudioSource>();

        if (this.actionCompleteSound != null && this.audioSource != null)
            this.audioSource.PlayOneShot(this.actionCompleteSound);
    }
}
