using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blob : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(spawn());
    }
    private IEnumerator spawn()
    {
        while (transform.localScale.x < 2)
        {
            transform.localScale += Vector3.one * Time.unscaledDeltaTime * 5f;
            GetComponentInChildren<Light2D>().intensity = Mathf.Lerp(GetComponentInChildren<Light2D>().intensity, 0.05f, 0.5f);
            if (transform.localScale.x >= 2)
            {
                transform.localScale = Vector3.one * 2;
                GetComponentInChildren<Light2D>().intensity = 0.05f;
            }
            yield return null;
        }
    }
    public void Collect()
    {
        StartCoroutine(collect());
    }
    private IEnumerator collect()
    {
        while (transform.localScale.x > 0)
        {
            GetComponent<Collider2D>().enabled = false;
            transform.localScale -= Vector3.one * Time.unscaledDeltaTime * 3f;
            GetComponentInChildren<Light2D>().intensity /= 2;
            if (transform.localScale.x <= 0)
            {
                transform.localScale = Vector3.zero;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
