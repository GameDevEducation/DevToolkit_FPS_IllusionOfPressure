using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Transform WeaponRoot;
    [SerializeField] GameObject ProjectilePrefab;
    [SerializeField] Transform ProjectileSpawnPoint;
    [SerializeField] TextMeshProUGUI FeedbackLabel;

    Projectile LinkedProjectile;
    public float PressureApplied
    {
        get
        {
            return LinkedProjectile.Damage / LinkedProjectile.FireInterval;
        }
    }

    public bool CanHitTarget = true;
    public bool ForceMiss = false;

    float TimeUntilNextFire = 0f;

    PlayerBase Player;

    Vector3 AimTarget
    {
        get
        {
            return ForceMiss ? (Player.HeadPosition + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f))) : Player.CentreOfMass;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        LinkedProjectile = ProjectilePrefab.GetComponent<Projectile>();
        Player = FindObjectOfType<PlayerBase>();
    }

    // Update is called once per frame
    void Update()
    {
        // update the feedback label
        FeedbackLabel.text = CanHitTarget ? (ForceMiss ? "Missing" : "Hitting") : "Can't Hit";

        // aim at our target
        WeaponRoot.LookAt(AimTarget, Vector3.up);

        // time to fire?
        TimeUntilNextFire -= Time.deltaTime;
        if (TimeUntilNextFire <= 0f && CanHitTarget)
            Fire();
    }

    void Fire()
    {
        // spawn the projectile
        var newProjectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.transform.position, WeaponRoot.transform.rotation);
        var projectileLogic = newProjectile.GetComponent<Projectile>();

        // set if the projectile is real or fake
        projectileLogic.FakeProjectile = ForceMiss;

        // set the cooldown
        TimeUntilNextFire = projectileLogic.FireInterval;
    }
}
