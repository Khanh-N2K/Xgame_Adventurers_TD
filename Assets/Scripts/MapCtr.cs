using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtr : Singleton<MapCtr>
{
    public Base[] enemyPrefab;
    public Base[] characterPrefab;

    public List<Base> listEnemys = new();
    public List<Base> listCharacters = new();

    public Transform characterTransform;
    public Transform enemyTransform;

    public SpawnSO spawnSOData;

    private int currentWave = 0;

    [Header("Spawn Setting")]
    [SerializeField] private int maxPerRow = 5;
    [SerializeField] private float spacingZ = 0.03f;
    [SerializeField] private float spacingX = 0.08f;
    [SerializeField] private float enemyStartX = 0.4f;
    [SerializeField] private float characterStartX = -0.55f;
    [SerializeField] private float posY = 0.03f;

    private int enemySpawnIndex = 0;
    private int characterSpawnIndex = 0;

    private void Start()
    {
        UIInGame.Instance.DoStart(() => StartCoroutine(SpawnEnemyWave(0)));
    }

    #region ==== GET CLOSEST ====

    public Base GetClosestEnemy(Vector3 root)
    {
        Base closest = null;
        float minDis = float.MaxValue;

        foreach (var enemy in listEnemys)
        {
            if (enemy == null) continue;
            if (enemy.status == Status.Die) continue;

            float dis = Vector3.SqrMagnitude(enemy.transform.position - root);
            if (dis < minDis)
            {
                minDis = dis;
                closest = enemy;
            }
        }
        return closest;
    }

    public Base GetClosestCharacter(Vector3 root)
    {
        Base closest = null;
        float minDis = float.MaxValue;

        foreach (var character in listCharacters)
        {
            if (character == null) continue;
            if (character.status == Status.Die) continue;

            float dis = Vector3.SqrMagnitude(character.transform.position - root);
            if (dis < minDis)
            {
                minDis = dis;
                closest = character;
            }
        }
        return closest;
    }

    #endregion

    #region ==== SPAWN CHARACTER ====

    public void SpawnCharacter(int id)
    {
        Base character = CreateCharacterObject(id, characterTransform);
        if (character == null) return;

        character.SwitchStatus(Status.Attack);
        listCharacters.Add(character);
        // for (int i = 0; i < 5; i++)
        // {
        // }
    }

    #endregion

    #region ==== SPAWN ENEMY ====

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnEnemyWave(currentWave));
    }

    public IEnumerator SpawnEnemyWave(int index)
    {
        enemySpawnIndex = 0;
        characterSpawnIndex = 0;

        InfoWave infoWave = spawnSOData.infoSpawn.infoWave[index];
        yield return new WaitForSeconds(infoWave.timeDelay);

        for (int i = 0; i < infoWave.infoTurns.Length; i++)
        {
            InfoTurn infoTurn = infoWave.infoTurns[i];

            for (int j = 0; j < infoTurn.enemy.Length; j++)
            {
                Base enemy = CreateEnemyObject(infoTurn.enemy[j], enemyTransform);
                listEnemys.Add(enemy);
            }

            yield return new WaitForSeconds(infoTurn.timedelay);
        }
    }

    #endregion

    #region ==== CREATE OBJECT ====

    private Vector3 GetEnemySpawnPos()
    {
        int row = enemySpawnIndex / maxPerRow;
        int col = enemySpawnIndex % maxPerRow;

        float x = enemyStartX + row * spacingX;
        float z = (col - (maxPerRow - 1) / 2f) * spacingZ;

        enemySpawnIndex++;
        return new Vector3(x, posY, z);
    }

    private Vector3 GetCharacterSpawnPos()
    {
        int row = characterSpawnIndex / maxPerRow;
        int col = characterSpawnIndex % maxPerRow;

        float x = characterStartX - row * spacingX;
        float z = (col - (maxPerRow - 1) / 2f) * spacingZ;

        characterSpawnIndex++;
        // return new Vector3(x, posY, z);
        float[] posZ = new float[] { -0.06f, -0.03f, 0f, 0.03f, 0.06f };
        int index = Random.Range(0, posZ.Length);
        return new Vector3(-0.64f, posY, posZ[index]);
    }

    public Base CreateEnemyObject(int id, Transform parent)
    {
        Base enemy = GetEnemyById(id);
        if (enemy == null) return null;

        Base newEnemy = Instantiate(enemy, GetEnemySpawnPos(), Quaternion.identity, parent);
        return newEnemy;
    }

    public Base CreateCharacterObject(int id, Transform parent)
    {
        Base character = GetCharacterById(id);
        if (character == null) return null;

        Base newCharacter = Instantiate(character, GetCharacterSpawnPos(), Quaternion.identity, parent);
        return newCharacter;
    }

    #endregion

    #region ==== UTILITY ====

    private Base GetEnemyById(int id)
    {
        if (id < 0 || id >= enemyPrefab.Length) return null;
        return enemyPrefab[id];
    }

    private Base GetCharacterById(int id)
    {
        if (id < 0 || id >= characterPrefab.Length) return null;
        return characterPrefab[id];
    }

    public bool CheckEndWave()
    {
        foreach (var e in listEnemys)
        {
            if (e == null) return false;
            if (e.status != Status.Die) return false;
        }

        // foreach (var c in listCharacters)
        //     if (c.status != Status.Die) return false;

        currentWave++;
        if (currentWave > 3) return false;
        StopAllCoroutines();
        StartCoroutine(SpawnEnemyWave(currentWave));

        return true;
    }

    #endregion

    #region ==== CONTEXT MENU ====

    [ContextMenu("Test All Attack")]
    private void TestAllAttack()
    {
        foreach (var enemy in listEnemys)
            enemy.SwitchStatus(Status.Attack);

        foreach (var character in listCharacters)
            character.SwitchStatus(Status.Attack);
    }

    #endregion
}
