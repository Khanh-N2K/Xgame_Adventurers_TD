using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIInGame : Singleton<UIInGame>
{
    public int Hp = 1000;
    public Text hpTxt;
    public Image fillHp;
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
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