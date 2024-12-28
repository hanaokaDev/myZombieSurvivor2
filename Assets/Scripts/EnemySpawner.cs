using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour {
    public Enemy enemyPrefab; // 생성할 적 AI

    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float damageMax = 40f; // 최대 공격력
    public float damageMin = 20f; // 최소 공격력

    public float healthMax = 200f; // 최대 체력
    public float healthMin = 100f; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int wave; // 현재 웨이브

    private void Update() {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 적을 모두 물리친 경우 다음 스폰 실행
        if (enemies.Count <= 0)
        {
            SpawnWave();
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave() {
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 1.5f); // 웨이브의 1.5배를 반올림한 수만큼 적 생성
        for (int i = 0; i < spawnCount; i++)
        {
            float enemyIntensity = Random.Range(0f, 1f); // 적의 세기를 0%에서 100% 사이로 조정
            // 적 생성
            CreateEnemy(enemyIntensity);
        }
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy(float intensity) {
        float health = Mathf.Lerp(healthMin, healthMax, intensity); // 체력
        float damage = Mathf.Lerp(damageMin, damageMax, intensity); // 공격력
        float speed = Mathf.Lerp(speedMin, speedMax, intensity); // 이동 속도
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity); // 피부색은 흰색에서 강해질수록 붉게
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // 소환 위치
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation); // 적 생성
        enemy.Setup(health, damage, speed, skinColor); // 적 설정
        enemies.Add(enemy); // 생성된 적을 리스트에 추가
        enemy.onDeath += () => OnEnemyDeath(enemy);
        enemy.onDeath += () => DestroyEnemy(enemy);
        enemy.onDeath += () => AddScore();
    }

    private void OnEnemyDeath(Enemy enemy) {
        enemies.Remove(enemy);
    }

    private void DestroyEnemy(Enemy enemy) {
        Destroy(enemy.gameObject, 10f);
    }

    private void AddScore() {
        GameManager.instance.AddScore(100);
    }
}