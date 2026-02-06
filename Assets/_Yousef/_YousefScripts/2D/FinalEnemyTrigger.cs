using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemySpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] float yOffset = 8f;
    [SerializeField] bool spawnAbovePlayer = true;
    [SerializeField] bool randomizeXInsideTrigger = true;

    [Header("Audio")]
    [SerializeField] AudioClip spawnSound;

    Collider2D triggerCollider;

    Vector3 savedSpawnPosition;
    bool spawnPositionSaved;

    GameObject currentEnemy;
    public GameObject CurrentEnemy => currentEnemy;

    void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (currentEnemy != null) return;

        if (!spawnPositionSaved)
        {
            savedSpawnPosition = CalculateSpawnPosition(other.transform);
            spawnPositionSaved = true;
        }

        currentEnemy = Instantiate(enemyPrefab, savedSpawnPosition, Quaternion.identity);

        if (spawnSound != null)
            AudioSource.PlayClipAtPoint(spawnSound, transform.position);
    }

    Vector3 CalculateSpawnPosition(Transform player)
    {
        Vector3 pos = player.position;
        pos.y += (spawnAbovePlayer ? 1f : -1f) * yOffset;

        if (randomizeXInsideTrigger)
        {
            Bounds b = triggerCollider.bounds;
            pos.x = Random.Range(b.min.x, b.max.x);
        }

        pos.z = 0f;
        return pos;
    }
}