using UnityEngine;
using TMPro;
using System;

public class CollectablesScript : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float volume = 1f;

    private Vector3 startPosition;

    private static int collectedCount;
    private static int totalCount;
    private static TextMeshProUGUI collectableText;

    public static Action OnAllCollected;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);

        collectedCount++;

        if (collectableText != null)
            collectableText.text = collectedCount + " / " + totalCount;

        if (collectedCount >= totalCount)
            OnAllCollected?.Invoke();

        Destroy(gameObject);
    }

    public static void SetupUI(TextMeshProUGUI textUI, int total)
    {
        collectableText = textUI;
        totalCount = total;
        collectedCount = 0;

        if (collectableText != null)
            collectableText.text = "0 / " + totalCount;
    }
}
