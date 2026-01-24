using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIInGame : Singleton<UIInGame>
{
    public int Coin = 50;
    public Text coinTxt;
    public CardSpawn[] listCards;
    public float timeBounsCoin = 5;
    public void Start()
    {
        coinTxt.text = Coin.ToString();
        for (int i = 0; i < listCards.Length; i++)
        {
            listCards[i].AddListener(() => SpawnCharacter(i));
        }
    }
    public void Update()
    {
        timeBounsCoin -= Time.deltaTime;
        if (timeBounsCoin <= 0)
        {
            timeBounsCoin = 5;
            AddCoin(3);
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
    public void CheckClickListCard()
    {
        foreach (CardSpawn card in listCards)
        {
            card.CheckClickButton();
        }
    }
}