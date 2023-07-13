using UnityEngine;

public class Spawner : MonoBehaviour
{
    // ���� ��ġ 
    public Transform[] spawnPoint;
    // ���� ������
    public SpawnData[] spawnData;

    public float levelTime;

    // ���� ��������
    int level;
    // ��ȯ �ð�
    float timer;

    private void Awake()
    {
        // ���� ����Ʈ ���� �޾ƿ���
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // �ð����� ���� �����ϰ� üũ
        timer += Time.deltaTime;
        level = Mathf.Min(
            Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        // ������ ���� ��ȯ �ӵ� ����
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

        transform.Rotate(Vector3.forward * Time.deltaTime);
    }

    // ����
    void Spawn()
    {
        // ������ ���� ���� ��ȯ ���� 
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
