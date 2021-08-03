using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public int Damage = 20;
    public float FireInterval = 0f;
    public float LaunchForce = 20f;
    public bool FakeProjectile = false;

    Rigidbody ProjectileRB;

    // Start is called before the first frame update
    void Start()
    {
        ProjectileRB = GetComponent<Rigidbody>();
        ProjectileRB.AddForce(LaunchForce * transform.forward, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (!FakeProjectile && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<PlayerBase>().TakeDamage(Damage);
        }

        Destroy(gameObject);
    }
}
