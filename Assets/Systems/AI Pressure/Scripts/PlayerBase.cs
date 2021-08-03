using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] Transform HeadRoot;
    [SerializeField] Transform BodyRoot;
    [SerializeField] float StatUpdateInterval = 0.2f;
    [SerializeField] float AverageTimeWindow = 10f;
    [SerializeField] TextMeshProUGUI AverageDPSDisplay;
    [SerializeField] PostProcessVolume EffectPPV;
    [SerializeField] float DamageForMaxEffect = 30f;
    [SerializeField] float VignetteSlewRate = 2f;

    public Vector3 HeadPosition => HeadRoot.position;
    public Vector3 CentreOfMass => BodyRoot.position;
    
    int TotalDamage = 0;
    List<Vector2> DamageHistory = new List<Vector2>();

    float StatUpdateCooldown = 0f;

    Vignette VignetteSettings;
    float TargetVignetteIntensity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        EffectPPV.sharedProfile.TryGetSettings<Vignette>(out VignetteSettings);
    }

    // Update is called once per frame
    void Update()
    {
        // time to update stats?
        StatUpdateCooldown -= Time.deltaTime;   
        if (StatUpdateCooldown <= 0f)
        {
            StatUpdateCooldown = StatUpdateInterval;

            // first remove old stats
            float thresholdTime = Time.time - AverageTimeWindow;
            float damageSum = 0f;
            float averageDPS = 0f;
            for (int index = 0; index < DamageHistory.Count; ++index)
            {
                // remove this sample?
                if (DamageHistory[index].x < thresholdTime)
                {
                    DamageHistory.RemoveAt(index);
                    --index;
                    continue;
                }

                damageSum += DamageHistory[index].y;
            }

            // is there a valid average?
            if (damageSum > 0f)
                averageDPS = damageSum / (Time.time - DamageHistory[0].x);

            // update the display
            AverageDPSDisplay.text = Mathf.RoundToInt(averageDPS).ToString() + " per second";

            // updatte the vignette intensity
            TargetVignetteIntensity = Mathf.Clamp01(averageDPS / DamageForMaxEffect);
        }

        // slew the vignette intensity
        VignetteSettings.intensity.value = Mathf.MoveTowards(VignetteSettings.intensity.value, TargetVignetteIntensity, VignetteSlewRate * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        TotalDamage += amount;
        DamageHistory.Add(new Vector2(Time.time, amount));
    }
}
