using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    public Transform powerupSpawn;
    public Vector3 powerupOff;
    public Transform powerupParent;
    public static Powerups instance;
    public int powerups;
    public int slowMos = 0;
    public int luckMos = 0;
    public PowerupDisplay lastSlow;
    public PowerupDisplay lastLuck;
    private bool ongoing = false;
    private string toAdd = "";
    private float toAddD = 0;
    void Awake()
    {
        instance = this;
    }
    public void AddPowerup(string name, float duration)
    {
        if (ongoing) toAdd = name;
        if (ongoing) toAddD = duration;
        if (ongoing) return;
        float delay = 0;
        PowerupDisplay display = Instantiate(Resources.Load("Prefabs/Powerup") as GameObject).GetComponent<PowerupDisplay>();
        if (name == "Slow Mo")
        {
            slowMos++;
            if (slowMos > 1)
            {
                delay += lastSlow.delay + lastSlow.time;
                duration = 2f;
            }
        }
        if (name == "Luck")
        {
            luckMos++;
            if (luckMos > 1)
            {
                delay += lastLuck.delay + lastLuck.time;
            }
        }
        display.transform.position = powerupSpawn.transform.position;
        display.transform.SetParent(powerupParent, true);
        display.Initiate(duration, name);
        display.addDelay(delay);
        if (name == "Slow Mo")
        {
            lastSlow = display;
        }
        if (name == "Luck")
        {
            lastLuck = display;
        }
        powerupSpawn.transform.position += powerupOff;
        powerups++;
    }
    public void Update()
    {
        if (!ongoing)
        {
            if (toAdd != "")
            {
                AddPowerup(toAdd, toAddD);
                toAdd = "";
                toAddD = 0;
            }
        }
    }
    public void Shift()
    {
        powerups--;
        powerupSpawn.transform.position -= powerupOff;
        StartCoroutine(shift());
    }
    private IEnumerator shift()
    {
        ongoing = true;
        float t = 0;
        Vector3 target = powerupParent.transform.position - powerupOff;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            powerupParent.transform.position = new Vector3(powerupParent.transform.position.x, Mathf.Lerp(powerupParent.transform.position.y, target.y, 0.4f), powerupParent.transform.position.z);
            yield return null;
        }
        powerupParent.transform.position = new Vector3(powerupParent.transform.position.x, target.y, powerupParent.transform.position.z);
        ongoing = false;
    }
}
