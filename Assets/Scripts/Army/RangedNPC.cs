using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedNPC : Base
{
    protected override void HandleAttack()
    {
        base.HandleAttack();

        FindTargetThenHandleAction();
    }

    private void FindTargetThenHandleAction()
    {
        Base target = GetClosestTarget();

        if (target == null)
        {
            SwitchStatus(Status.Idle);
        }
        else
        {
            SetTarget(target);
            StartCoroutine(AttackCurrentTargetIE());
        }
    }

    private IEnumerator AttackCurrentTargetIE()
    {
        while (target.currenHeath > 0)
        {
            yield return new WaitForSeconds(info.attackDelay);

            if (target == null || target.status == Status.Die)
            {
                target = null;
                SwitchStatus(Status.Attack);
                yield break;
            }
            else
            {
                target.TakeDamage(info.damage);
            }

            yield return null;
        }
    }
}
