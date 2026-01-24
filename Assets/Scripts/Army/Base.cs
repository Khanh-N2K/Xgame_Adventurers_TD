using System;
using System.Collections;
using UnityEngine;
public class Base : MonoBehaviour
{
    [Header("Status")]

    public Status status;

    public bool isEnemy;

    [Header("Info")]

    public Info info;
    public int currenHeath;
    public int maxHealth;

    [Header("Animator")]

    public Animator animator;
    public string idleAnimName = "idle";
    public string moveAnimName = "move";
    public string attackeAnimName = "attack";
    public string dieAnimName = "die";
    public float delayDestroyTime = 2f;

    [Header("Target")]

    public Base target;

    private void Awake()
    {
        SetUp();
        SwitchStatus(Status.Idle);
    }

    public virtual void SetUp()
    {
        currenHeath = info.heath;
        maxHealth = info.heath;
    }

    public virtual void SwitchStatus(Status status)
    {
        StopAllCoroutines();

        this.status = status;
        switch (status)
        {
            case Status.Idle:
                HandleIdle();
                break;
            case Status.Attack:
                HandleAttack();
                break;
            case Status.Die:
                HandleDie();
                break;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currenHeath -= damage;
        if (currenHeath < 0)
        {
            currenHeath = 0;
            SwitchStatus(Status.Die);
        }
    }

    public virtual void SetTarget(Base target)
    {
        this.target = target;
    }

    public void PlayAnimation(string animName)
    {
        // if (animator.HasState(0, Animator.StringToHash(animName)))
        // {
        //     animator.CrossFade(animName, 0.1f);
        // }
    }

    protected Base GetClosestTarget()
    {
        Base target;
        if (isEnemy)
        {
            target = MapCtr.Instance.GetClosestCharacter(transform.position);
        }
        else
        {
            target = MapCtr.Instance.GetClosestEnemy(transform.position);
        }
        return target;
    }

    #region ___ STATUS ___

    protected void HandleIdle()
    {
        PlayAnimation(idleAnimName);
    }

    protected virtual void HandleAttack()
    {

    }

    protected virtual void HandleDie()
    {
        if (isEnemy)
        {
            MapCtr.Instance.listEnemys.Remove(this);
        }
        else
        {
            MapCtr.Instance.listCharacters.Remove(this);
        }

        PlayAnimation(dieAnimName);

        Invoke(nameof(DestroySelf), delayDestroyTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion ___
}