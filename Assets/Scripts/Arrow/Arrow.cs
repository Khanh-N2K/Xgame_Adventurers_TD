using DG.Tweening;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public ParticleSystem explosionFX;

    public float speed = 15f;
    public float arcHeight;

    private Vector3 startPos;
    private Vector3 lastPos;
    private Transform target;

    private float travelTime;
    private float timer;

    public void Init(Transform target)
    {
        this.target = target;
        startPos = transform.position;
        lastPos = startPos;

        float distance = Vector3.Distance(startPos, target.position);
        timer = 0;
        travelTime = distance / speed;

        if(explosionFX != null)
        {
            explosionFX.Stop();
            explosionFX.Clear();
        }
    }

    void Update()
    {
        if (target == null)
        {
            HitTarget();
            return;
        }

        timer += Time.deltaTime;
        float t = timer / travelTime;

        if (t >= 1f)
        {
            PlayFX();
            HitTarget();
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPos, target.position, t);

        // tạo đường cong
        arcHeight = Vector3.Distance(startPos, target.position) * 1.5f;
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * t) * 0.1f;

        transform.position = currentPos;

        // rotate theo hướng di chuyển
        Vector3 velocity = (currentPos - lastPos) / Time.deltaTime;
        if (velocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
        lastPos = currentPos;
    }

    void HitTarget()
    {
        // TODO: Damage target

        Destroy(gameObject);
    }

    private void PlayFX()
    {
        if (explosionFX == null)
        {
            return;
        }
        ParticleSystem fx = Instantiate(explosionFX, transform.position + Vector3.up * 0.03f, Quaternion.identity);
        fx.transform.SetParent(null);
        fx.Play();
    }
}
