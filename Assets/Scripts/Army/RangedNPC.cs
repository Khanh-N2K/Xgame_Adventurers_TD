using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedNPC : Base
{
    [Header("Archer Settings")]
    public GameObject arrowPrefab;
    public Transform shootPoint;

        protected override void PerformAttackAction()
        {
            base.PerformAttackAction();

            if (target != null && arrowPrefab != null && shootPoint != null)
            {
                GameObject arrow = Instantiate(
                    arrowPrefab,
                    shootPoint.position,
                    shootPoint.rotation
                );

                // ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();
                // if (projectile != null)
                // {
                //     projectile.Init(target.transform);
                // }
            }
        }

}
