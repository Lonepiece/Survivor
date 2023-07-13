using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 스폰 위치 
    public Transform[] spawnPoint;
    // 스폰 데이터
    public SpawnData[] spawnData;

    public float levelTime;

    // 현재 스테이지
    int level;
    // 소환 시간
    float timer;

    private void Awake()
    {
        // 스폰 포인트 전부 받아오기
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 시간마다 레벨 증가하게 체크
        timer += Time.deltaTime;
        level = Mathf.Min(
            Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        // 레벨에 따라 소환 속도 증가
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

        transform.Rotate(Vector3.forward * Time.deltaTime);
    }

    // 생성
    void Spawn()
    {
        // 레벨에 따라 몬스터 소환 변경 
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
