using System;
using UnityEngine;
public class Base : MonoBehaviour
{
    public Status status;

    public Info info;
    public int currenHeath;
    
    public AnimationStatus animationStatus;
    public Animator animator;
    public string idleAnimName = "idle";
    public string moveAnimName = "move";
    public string attackeAnimName = "attack";

    public Base target;

    public virtual void SetUp()
    {
        currenHeath = info.heath;
    }

    public virtual void switchStatus(Status status)
    {
        this.status = status;
        animationStatus.SwitchAnim(status);

        switch (status)
        {
            case Status.Idle:
                HandleIdle();
                break;
            case Status.Move:
                HandleMove();
                break;
            case Status.Attack:
                HandleAttack();
                break;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        
    }

    public virtual void SetTarget(Base target)
    {
        this.target = target;
    }

    public void PlayAnimation(string animName)
    {
        animator.CrossFade(animName, 0.1f);
    }

    public void MoveTo(Vector3 targetPos)
    {

    }

    #region ___ STATUS ___

    public void HandleIdle()
    {
        PlayAnimation(idleAnimName);
    }

    public void HandleMove()
    {
        PlayAnimation(moveAnimName);
    }

    public void HandleAttack()
    {
        PlayAnimation(attackeAnimName);
    }

    #endregion ___
}