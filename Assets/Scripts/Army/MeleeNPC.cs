using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MeleeNPC : Base
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
            StartCoroutine(MoveToIE(target.transform.position, onFinished: () => FindTargetThenHandleAction()));
        }
    }

    public IEnumerator MoveToIE(Vector3 targetPos, Action onFinished)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            // Check if has close target to attack
            Base target = GetClosestTarget();
            if (target != null && Vector3.SqrMagnitude(target.transform.position - transform.position) < info.range * info.range)
            {
                SetTarget(target);
                yield return AttackCurrentTargetIE();
                yield break;
            }

            // Else move normal
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                info.speed * Time.deltaTime
            );

            yield return null;
        }

        // Snap exactly to target
        transform.position = targetPos;

        onFinished?.Invoke();
    }

    private IEnumerator AttackCurrentTargetIE()
    {
        while(target.currenHeath > 0)
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
