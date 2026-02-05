using System;
using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    #region ___ STATUS ___

    [Header("Status")]
    public Status status;
    public bool isEnemy;

    #endregion

    #region ___ INFO ___

    [Header("Info")]
    public Info info;
    public int currenHeath;
    public int maxHealth;
    public SpriteRenderer fill;
    #endregion

    #region ___ ANIMATOR ___

    [Header("Animator")]
    public Animator animator;
    public string idleAnimName = "idle";
    public string moveAnimName = "move";
    public string attackAnimName = "attack";
    public string dieAnimName = "die";
    public float delayDestroyTime = 2f;

    #endregion

    #region ___ TARGET ___

    [Header("Target")]
    [SerializeField] protected float stopDistance = 0.1f;
    [SerializeField] protected float rotationSpeed = 10f;
    public Base target;

    #endregion

    #region ___ UNITY ___

    private void Awake()
    {
        SetUp();
        SwitchStatus(Status.Idle);
    }

    #endregion

    #region ___ SETUP ___

    public virtual void SetUp()
    {
        currenHeath = info.heath;
        maxHealth = info.heath;
    }

    #endregion

    #region ___ FSM ___

    public virtual void SwitchStatus(Status newStatus)
    {
        StopAllCoroutines();
        status = newStatus;

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

            case Status.Die:
                HandleDie();
                break;
        }
    }

    #endregion

    #region ___ COMBAT ___

    public virtual void TakeDamage(int damage)
    {
        if (status == Status.Die) return;

        currenHeath -= damage;
        FillSprite((float)currenHeath / info.heath, 0.01f, true);
        if (currenHeath <= 0)
        {
            currenHeath = 0;
            SwitchStatus(Status.Die);
        }
    }

    public virtual void SetTarget(Base target)
    {
        this.target = target;
    }

    #endregion

    #region ___ ANIMATION ___

    public void PlayAnimation(string animName)
    {
        if (animator != null &&
            animator.HasState(0, Animator.StringToHash(animName)))
        {
            animator.CrossFade(animName, 0.1f);
        }
    }

    #endregion

    #region ___ TARGETING ___

    protected Base GetClosestTarget()
    {
        if (isEnemy)
            return MapCtr.Instance.GetClosestCharacter(transform.position);
        else
            return MapCtr.Instance.GetClosestEnemy(transform.position);
    }

    #endregion

    #region ___ MOVE ___

    protected virtual void HandleMove()
    {
        Vector3 destination = new Vector3(-0.65f, transform.position.y, transform.position.z);
        StartCoroutine(MoveToPositionIE(destination));
    }

    protected IEnumerator MoveToPositionIE(Vector3 pos)
    {
        PlayAnimation(moveAnimName);

        while (Vector3.Distance(transform.position, pos) > stopDistance)
        {
            // Check if in range attack 
            Base target = GetClosestTarget();
            if (target != null && Vector3.SqrMagnitude(target.transform.position - transform.position) < info.range * info.range)
            {
                SetTarget(target);
                SwitchStatus(Status.Attack);
                yield break;
            }

            // Move toward position
            transform.position = Vector3.MoveTowards(
                transform.position,
                pos,
                info.speed * Time.deltaTime
            );

            // Rotate toward movement direction (Y axis only)
            Vector3 direction = pos - transform.position;
            RotateTowardDirection(direction);

            yield return null;
        }

        SwitchStatus(Status.Idle);
    }

    protected IEnumerator MoveToTargetIE(Vector3 targetPos, Action onFinished)
    {
        PlayAnimation(moveAnimName);

        // Preserve Y position for planar movement
        Vector3 destination = new Vector3(targetPos.x, transform.position.y, targetPos.z);

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            // Check if has close target to attack
            Base currentTarget = GetClosestTarget();
            if (currentTarget != null && Vector3.SqrMagnitude(currentTarget.transform.position - transform.position) < info.range * info.range)
            {
                SetTarget(currentTarget);
                yield return AttackTargetIE();
                yield break;
            }

            // Move toward destination
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                info.speed * Time.deltaTime
            );

            // Rotate toward movement direction (Y axis only)
            Vector3 direction = destination - transform.position;
            RotateTowardDirection(direction);

            yield return null;
        }

        // Snap exactly to target
        transform.position = destination;
        onFinished?.Invoke();
    }

    protected void RotateTowardDirection(Vector3 direction)
    {
        // Only rotate on Y axis (planar rotation)
        if (direction.sqrMagnitude < 0.001f) return;

        // Zero out Y component for planar rotation
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    protected void RotateTowardTarget(Base targetBase)
    {
        if (targetBase == null) return;

        Vector3 direction = targetBase.transform.position - transform.position;
        RotateTowardDirection(direction);
    }

    #endregion

    #region ___ STATE HANDLERS ___

    protected virtual void HandleIdle()
    {
        PlayAnimation(idleAnimName);

        // Rotate toward target if we have one
        if (target != null && status == Status.Idle)
        {
            RotateTowardTarget(target);
        }

        // Enemy auto-move decision
        if (isEnemy)
        {
            if (Vector3.Distance(transform.position, new Vector3(-0.56f, transform.position.y, transform.position.z)) > 0.5f)
            {
                SwitchStatus(Status.Move);
            }
            else
            {
                StartCoroutine(AttackFortressIE());
            }
        }
        else
        {
            StartCoroutine(DelayAttackIE());
        }
    }

    private IEnumerator DelayAttackIE()
    {
        yield return new WaitForSeconds(0.5f);
        SwitchStatus(Status.Attack);
    }

    protected virtual void HandleAttack()
    {
        PlayAnimation(attackAnimName);
        FindTargetThenHandleAction();
    }

    protected IEnumerator AttackFortressIE()
    {
        while (true)
        {
            // Call the attack action (can be overridden for different attack types)
            PerformAttackAction();

            yield return new WaitForSeconds(info.attackDelay);

            UIInGame.Instance.TakeDamage(info.damage);

            yield return null;
        }
    }

    private void FindTargetThenHandleAction()
    {
        Base foundTarget = GetClosestTarget();
        if (foundTarget == null)
        {
            SwitchStatus(Status.Idle);
        }
        else
        {
            StartCoroutine(MoveToTargetIE(foundTarget.transform.position, onFinished: () => FindTargetThenHandleAction()));
        }
    }

    protected IEnumerator AttackTargetIE()
    {
        while (target != null && target.currenHeath > 0)
        {
            // Rotate toward target during attack
            RotateTowardTarget(target);

            // Call the attack action (can be overridden for different attack types)
            PerformAttackAction();

            yield return new WaitForSeconds(info.attackDelay);

            // Validate target still exists
            if (!IsTargetValid())
            {
                target = null;
                SwitchStatus(Status.Attack);
                yield break;
            }

            // Deal damage
            target.TakeDamage(info.damage);

            yield return null;
        }

        // Target defeated, find next target
        target = null;
        SwitchStatus(Status.Attack);
    }

    protected bool IsTargetValid()
    {
        if (target == null || target.status == Status.Die)
            return false;

        if (isEnemy)
            return MapCtr.Instance.listCharacters.Contains(target);
        else
            return MapCtr.Instance.listEnemys.Contains(target);
    }

    // Override this method in child classes for different attack animations/effects
    protected virtual void PerformAttackAction()
    {
        PlayAnimation(attackAnimName);
        // Child classes can add specific attack effects here
        // e.g., MeleeNPC: sword swing effect
        // e.g., ArcherNPC: shoot arrow projectile
    }

    protected virtual void HandleDie()
    {
        if (isEnemy)
        {
            MapCtr.Instance.listEnemys.Remove(this);
            MapCtr.Instance.CheckEndWave();
            UIInGame.Instance.AddCoin(1);
        }
        else
            MapCtr.Instance.listCharacters.Remove(this);

        PlayAnimation(dieAnimName);
        Invoke(nameof(DestroySelf), delayDestroyTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void FillSprite(float percent, float fullScaleX, bool anchorLeft = true)
    {
        if (fill == null) return;
        percent = Mathf.Clamp01(percent);

        Vector3 scale = fill.transform.localScale;
        scale.x = fullScaleX * percent;
        fill.transform.localScale = scale;

        if (anchorLeft)
        {
            float offset = (fullScaleX - scale.x) * 0.5f;
            Vector3 pos = fill.transform.localPosition;
            pos.x = -offset;
            fill.transform.localPosition = pos;
        }
    }
    #endregion
}