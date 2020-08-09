using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] protected AudioClip actionSound = null;
    [SerializeField] private GameObject particles = null;
    [SerializeField] protected string affectedObjectTag;
    private AudioSource audioSource = null;

    public virtual void AffectObject(Transform target) { }

    private void Start()
    {
        this.audioSource = Camera.main.GetComponent<AudioSource>();
    }

    public void SetParticleActive(bool active)
    {
        this.particles.SetActive(active);
    }

    protected void PlayCompleteSound()
    {
        if (this.actionSound == null || this.audioSource == null)
            return;

        this.audioSource.PlayOneShot(this.actionSound);
    }
}
