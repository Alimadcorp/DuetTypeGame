using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class PowerupDisplay : MonoBehaviour
{
    public TextMeshPro text;
    public GameObject fill;
    public float time;
    public float percentage;
    private bool ended = false;
    public int slotIndex;
    private string id = "";
    public void Initiate(float t, string name)
    {
        time = t;
        text.text = name;
        id = name;
        percentage = 1;
    }
    void Update()
    {
        fill.transform.localScale = new Vector3(Mathf.Clamp01(percentage), 1, 1);
        percentage -= Time.deltaTime / time;
        if(percentage < 0 && !ended)
        {
            percentage = 0;
            ended = true;
            StartCoroutine(Remove());
        }
    }
    private IEnumerator Remove()
    {
        float t = 1;
        GameManager.Instance.RemovePowerup(id);
        while (t > 0)
        {
            t -= Time.deltaTime * 2;
            SpriteRenderer f = fill.GetComponentInChildren<SpriteRenderer>();
            f.color = new Color(f.color.r, f.color.g, f.color.b, t);
            text.color = new Color(text.color.r, text.color.g, text.color.b, t);
            yield return null;
        }
        Powerups.instance.Shift();
        Destroy(gameObject);
    }
}
