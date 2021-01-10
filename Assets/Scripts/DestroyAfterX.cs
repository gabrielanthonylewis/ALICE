using System.Collections;
using UnityEngine;

public class DestroyAfterX : MonoBehaviour
{
    [SerializeField] private float lifetime = 5.0f;

    private void Start()
    {
        this.StartCoroutine(this.DestroyAfter(this.lifetime));
    }

	private IEnumerator DestroyAfter(float delay)
	{
		yield return new WaitForSeconds(delay);

		GameObject.Destroy(this.gameObject);
	}
}
