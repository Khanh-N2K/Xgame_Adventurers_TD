using UnityEngine;
using UnityEngine.UI;

public class UIInGame : Singleton<UIInGame>
{
    public int Coin = 50;
    public CardSpawn[] listCards;
    public void Start()
    {
        for (int i = 0; i < listCards.Length; i++)
        {
            listCards[i].AddListener(() => SpawnCharacter(i));
        }
    }
    public void SpawnCharacter(int idx)
    {
        MapCtr.Instance.SpawnCharacter(idx);
    }
}