using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SquadController : MonoBehaviour
{
    [SerializeField] float MaxPlayerPressure = 20f;
    [SerializeField] float RebalanceInterval = 5f;
    [SerializeField] int NumNearestToInclude = 2;

    List<EnemyBase> Members = new List<EnemyBase>();
    float CountdownToRebalance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Members.AddRange(FindObjectsOfType<EnemyBase>());
    }

    // Update is called once per frame
    void Update()
    {
        // rebalance periodically
        CountdownToRebalance -= Time.deltaTime;
        if (CountdownToRebalance <= 0f)
        {
            Rebalance();
            CountdownToRebalance = RebalanceInterval;
        }
    }

    void Rebalance()
    {
        var target = FindObjectOfType<PlayerBase>();

        // get a list of AIs who can attack and order by increasing distance
        List<EnemyBase> candidateAttackers = Members.Where(member => member.CanHitTarget)
                                                    .OrderBy(member => Vector3.Distance(member.transform.position, target.transform.position))
                                                    .ToList();

        float currentPressure = 0f;

        // add in the nearest attackers
        for (int index = 0; index < NumNearestToInclude && index < candidateAttackers.Count; ++index)
        {
            candidateAttackers[index].ForceMiss = false;
            currentPressure += candidateAttackers[index].PressureApplied;
        }
        candidateAttackers.RemoveRange(0, Mathf.Min(NumNearestToInclude, candidateAttackers.Count));

        // pick random members to attack based on pressure
        while (candidateAttackers.Count > 0 && currentPressure < MaxPlayerPressure)
        {
            int randomIndex = Random.Range(0, candidateAttackers.Count);

            var candidatePressure = candidateAttackers[randomIndex].PressureApplied;

            // not a viable option - remove from attackers
            if ((currentPressure + candidatePressure) > MaxPlayerPressure)
            {
                candidateAttackers[randomIndex].ForceMiss = true;
            }
            else
            {
                // allow to attack
                candidateAttackers[randomIndex].ForceMiss = false;
                currentPressure += candidatePressure;
            }
            
            candidateAttackers.RemoveAt(randomIndex);
        }

        foreach(var member in candidateAttackers)
            member.ForceMiss = true;
    }
}
