using UnityEngine;

public class SpawnThunder : MonoBehaviour
{
    [SerializeField] private GameObject thunderPrefab;
    [SerializeField] private AudioClip thunderSound;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;

    private float timer;

    void Start()
    {
        timer = cooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Vector3 spawnPos = transform.position + new Vector3(xOffset, yOffset, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);

            Instantiate(thunderPrefab, spawnPos, rotation);

            AudioSource.PlayClipAtPoint(thunderSound, spawnPos, volume);

            timer = cooldown;
        }
    }
}
