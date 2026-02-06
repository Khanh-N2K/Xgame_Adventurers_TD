using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public Button btn;
    public Image fill;
    public Text text;
    public Transform skillPrefab;
    public Transform skillTransform;
    [SerializeField] private int maxPerRow = 5;
    [SerializeField] private float spacingZ = 0.05f;
    [SerializeField] private float spacingX = 0.05f;
    [SerializeField] private float posY = 0.025f;
    private int spawnIndex = 0;
    public bool isSkill;
    private void Awake()
    {
        isSkill = false;
        fill.fillAmount = 0;
        text.text = "0";
        text.gameObject.SetActive(false);
        btn.onClick.AddListener(OnSkill);
    }
    private void OnSkill()
    {
        if (isSkill) return;
        spawnIndex = 0;
        isSkill = true;
        btn.enabled = false;
        skillTransform.position = new Vector3(-1, 0, 0);
        List<Transform> prefabs = new List<Transform>();
        for (int i = 0; i < 12; i++)
        {
            Transform newPrefab = Instantiate(skillPrefab, GetSpawnPos(), Quaternion.identity);
            newPrefab.transform.SetParent(skillTransform, false);
            newPrefab.transform.eulerAngles = new Vector3(0, 90, 0);
            prefabs.Add(newPrefab);
        }
        HashSet<Base> damagedEnemies = new HashSet<Base>();

        foreach (Transform s in prefabs)
        {
            float posX = s.position.x;

            s.DOMoveX(posX + 5f, 10f)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    foreach (Base e in MapCtr.Instance.listEnemys)
                    {
                        if (e == null || e.status == Status.Die) continue;
                        if (damagedEnemies.Contains(e)) continue;

                        if (Mathf.Abs(e.transform.position.x - s.transform.position.x) < 0.1f)
                        {
                            e.TakeDamage(50);
                            damagedEnemies.Add(e);
                        }
                    }
                })
                .OnComplete(() =>
                {
                    Destroy(s.gameObject);
                });
        }
        StartCountdown(25);
    }
    private Vector3 GetSpawnPos()
    {
        int row = spawnIndex / maxPerRow;
        int col = spawnIndex % maxPerRow;

        float x = -(row * spacingX);
        float z = (col - (maxPerRow - 1) / 2f) * spacingZ;

        spawnIndex++;
        return new Vector3(x, posY, z);
    }
    public void StartCountdown(float duration)
    {
        text.text = duration.ToString();
        float valChange = duration;
        text.gameObject.SetActive(true);
        DOTween.To(() => valChange, x => valChange = x, 0, duration).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                text.text = Mathf.CeilToInt(valChange).ToString();
                fill.fillAmount = valChange / duration;
            })
            .OnComplete(() =>
            {
                isSkill = false;
                btn.enabled = true;
                fill.fillAmount = 0;
                text.text = "0";
                text.gameObject.SetActive(false);
                Debug.Log("⏰ Countdown finished");
            }).SetId(this);
    }
}