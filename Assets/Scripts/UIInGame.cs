using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIInGame : Singleton<UIInGame>
{
    public int Hp = 1000;
    public Text hpTxt;
    public Image fillHp;
    public int Coin = 50;
    public Text coinTxt;
    public CardSpawn[] listCards;
    public float timeBounsCoin = 5;
    public RectTransform VsRect;
    public RectTransform DichRect;
    public RectTransform QuanRect;
    public CanvasGroup canvasGroup;
    public bool IsStart;
    public void Start()
    {
        coinTxt.text = Coin.ToString();
        for (int i = 0; i < listCards.Length; i++)
        {
            int idx = i;
            listCards[i].AddListener(() => SpawnCharacter(idx));
        }
    }
    public void DoStart(Action cb)
    {
        IsStart = false;
        VsRect.DOAnchorPosY(0, 0.5f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            DichRect.DOAnchorPosX(0, 0.5f).SetDelay(0.1f).SetEase(Ease.InOutBack);
            QuanRect.DOAnchorPosX(0, 0.5f).SetDelay(0.1f).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                canvasGroup.DOFade(0, 0.5f).SetDelay(0.25f).OnComplete(() =>
                {
                    cb?.Invoke();
                    IsStart = true;
                });
            });
        });
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
        timeBounsCoin -= Time.deltaTime;
        if (timeBounsCoin <= 0)
        {
            timeBounsCoin = 5;
            AddCoin(30);
        }
    }
    public void SpawnCharacter(int idx)
    {
        MapCtr.Instance.SpawnCharacter(idx);
    }
    public void AddCoin(int amount)
    {
        int startValue = Coin;
        int endValue = Coin + amount;

        Coin = endValue;
        CheckClickListCard();
        DOTween.To(
            () => startValue,
            x =>
            {
                startValue = x;
                coinTxt.text = x.ToString();
            },
            endValue,
            0.5f
        ).SetEase(Ease.OutQuad).OnComplete(() => { coinTxt.text = Coin.ToString(); });
    }
    public void RemoveCoin(int amount)
    {
        int startValue = Coin;
        int endValue = Mathf.Max(0, Coin - amount);

        Coin = endValue;
        CheckClickListCard();
        DOTween.To(
            () => startValue,
            x =>
            {
                startValue = x;
                coinTxt.text = x.ToString();
            },
            endValue,
            0.5f
        ).SetEase(Ease.InQuad).OnComplete(() => { coinTxt.text = Coin.ToString(); });
    }
    public void TakeDamage(int damage)
    {
        int startValue = Hp;
        int endValue = Mathf.Max(0, Hp - damage);

        Hp = endValue;
        CheckClickListCard();
        DOTween.To(
            () => startValue,
            x =>
            {
                startValue = x;
                hpTxt.text = x.ToString();
                fillHp.fillAmount = x / 1000f;
            },
            endValue,
            0.5f
        ).SetEase(Ease.InQuad).OnComplete(() =>
        {
            hpTxt.text = Hp.ToString();
            fillHp.fillAmount = Hp / 1000f;
        });
    }
    public void CheckClickListCard()
    {
        foreach (CardSpawn card in listCards)
        {
            card.CheckClickButton();
        }
    }
}