using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DarknessTrigger : MonoBehaviour
{
    [SerializeField] DarknessController darknessController;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] bool triggerOnce = true;
    [SerializeField] string playerTag = "Player";
    [SerializeField] Canvas canvasToDisable;
    [SerializeField] GameObject gameObjectToEnable;
    [SerializeField] bool destroyAfterTrigger = true;
    [SerializeField] GameObject firstObjectToDestroy;
    [SerializeField] GameObject secondObjectToDestroy;

    [Header("Audio")]
    [SerializeField] AudioSource backgroundMusicSource;
    [SerializeField] AudioSource oneTimeSoundSource;
    [SerializeField] AudioClip newAudioClip;

    Collider2D col;
    bool hasTriggered = false;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || !other.CompareTag(playerTag)) return;
        hasTriggered = true;

        if (darknessController == null)
            darknessController = FindObjectOfType<DarknessController>();

        if (darknessController != null)
            darknessController.StartDarknessFade(fadeDuration, 1f, 1f);

        if (backgroundMusicSource != null && oneTimeSoundSource != null && newAudioClip != null)
            StartCoroutine(FadeOutAndPlayNew());

        if (canvasToDisable != null)
            canvasToDisable.enabled = false;

        if (gameObjectToEnable != null)
            gameObjectToEnable.SetActive(true);

        if (firstObjectToDestroy != null)
            Destroy(firstObjectToDestroy);

        if (secondObjectToDestroy != null)
            Destroy(secondObjectToDestroy);

        if (triggerOnce)
        {
            if (col != null) col.enabled = false;
        }

        if (destroyAfterTrigger)
        {
            Destroy(gameObject, fadeDuration + 0.1f);
        }
    }

    IEnumerator FadeOutAndPlayNew()
    {
        if (backgroundMusicSource == null) yield break;

        float startVolume = backgroundMusicSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            if (backgroundMusicSource == null) yield break;

            timer += Time.deltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = 0f;
            backgroundMusicSource.Stop();
        }

        if (oneTimeSoundSource != null && newAudioClip != null)
        {
            oneTimeSoundSource.PlayOneShot(newAudioClip);
        }
    }
}