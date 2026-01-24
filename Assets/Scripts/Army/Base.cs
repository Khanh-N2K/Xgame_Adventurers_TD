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
        StartCoroutine(MoveToIE(destination));
    }

    private IEnumerator MoveToIE(Vector3 pos)
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

            // Acquire target while moving
            if (target == null)
                target = GetClosestTarget();

            // Switch to attack if close to target
            if (target != null)
            {
                float dist = Vector3.Distance(
                    transform.position,
                    target.transform.position
                );

                if (dist <= stopDistance)
                {
                    SwitchStatus(Status.Attack);
                    yield break;
                }
            }

            yield return null;
        }

        SwitchStatus(Status.Idle);
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

    // Alternative: Rotate toward a specific target
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
        if (isEnemy && Vector3.Distance(transform.position, new Vector3(-0.65f, transform.position.y, transform.position.z)) > 0.5f)
        {
            SwitchStatus(Status.Move);
        }
    }

    protected virtual void HandleAttack()
    {
        PlayAnimation(attackAnimName);

        // Keep rotating toward target during attack
        if (target != null)
        {
            StartCoroutine(RotateTowardTargetCoroutine());
        }
    }

    private IEnumerator RotateTowardTargetCoroutine()
    {
        while (status == Status.Attack && target != null)
        {
            RotateTowardTarget(target);
            yield return null;
        }
    }

    protected virtual void HandleDie()
    {
        if (isEnemy)
            MapCtr.Instance.listEnemys.Remove(this);
        else
            MapCtr.Instance.listCharacters.Remove(this);

        PlayAnimation(dieAnimName);
        Invoke(nameof(DestroySelf), delayDestroyTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion
}