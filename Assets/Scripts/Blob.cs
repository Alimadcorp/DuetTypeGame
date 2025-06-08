using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Blob : MonoBehaviour
{
    public string id = "";
    public Color[] colorsForPowerups = new Color[3];
    public TextMeshPro view;
    public GameObject blot;
    public void MakePowerup(string _id)
    {
        id = _id;
        blot.GetComponent<SpriteRenderer>().color = colorsForPowerups[idToInt(id)];
        GetComponent<SpriteRenderer>().color = colorsForPowerups[idToInt(id)];
        GetComponentInChildren<Light2D>().color = colorsForPowerups[idToInt(id)];
        view.color = colorsForPowerups[idToInt(id)];
    }
    public static int idToInt(string _id)
    {
        switch (_id)
        {
            case "2x": return 0;
            case "4x": return 1;
            case "slow": return 2;
            default: return 0;
        }
    }
    public static string intToId(int n)
    {
        switch (n)
        {
            case 0: return "2x";
            case 1: return "4x";
            case 2: return "slow";
            default: return "";
        }
    }
    private void Start()
    {
        StartCoroutine(spawn());
        Invoke("despawn", 15f);
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
        GetComponent<Collider2D>().enabled = true;
    }
    public void Collect()
    {
        StartCoroutine(collect());
    }
    public void despawn()
    {
        StartCoroutine(Despawn());
    }
    private IEnumerator Despawn()
    {
        while (transform.localScale.x > 0)
        {
            GetComponent<Collider2D>().enabled = false;
            transform.localScale -= Vector3.one * Time.unscaledDeltaTime * 3f;
            GetComponentInChildren<Light2D>().intensity /= 1.08f;
            if (transform.localScale.x <= 0)
            {
                transform.localScale = Vector3.zero;
            }
            yield return null;
        }
        GameManager.Instance.blobAmt--;
        Destroy(gameObject);
    }
    private IEnumerator collect()
    {
        CancelInvoke();
        StopCoroutine(spawn());
        blot.transform.SetParent(null, true);
        if (blot != null) blot.transform.localScale = Vector3.one * 2;
        while (transform.localScale.x > 0)
        {
            GetComponent<Collider2D>().enabled = false;
            transform.localScale -= Vector3.one * Time.unscaledDeltaTime * 3f;
            GetComponentInChildren<Light2D>().intensity /= 1.08f;
            if (blot != null)
            {
                blot.transform.localScale += Vector3.one * Time.unscaledDeltaTime * 3f;
                blot.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, Time.unscaledDeltaTime * 3f);
                if (blot.GetComponent<SpriteRenderer>().color.a < 0)
                {
                    Destroy(blot);
                }
            }
            if (transform.localScale.x <= 0)
            {
                transform.localScale = Vector3.zero;
            }
            yield return null;
        }
        Destroy(blot);
        Destroy(gameObject);
    }
}
