using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EvilDarknessFade : MonoBehaviour
{
    [SerializeField] DarknessController darknessController;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] string playerTag = "Player";

    [Header("Player Stat Changes (SUBTRACT)")]
    [SerializeField] float speedToRemove;
    [SerializeField] float jumpForceToRemove;
    [SerializeField] float gravityScaleToRemove;

    [Header("Post-Fade Object Changes")]
    [SerializeField] GameObject objectToEnable;
    [SerializeField] GameObject objectToDestroy;

    [Header("Destroy Spawned Enemy Automatically")]
    [SerializeField] EnemySpawnTrigger spawnTrigger;

    [Header("Reset Player Health After Fade")]
    [SerializeField] bool resetPlayerHealth = true;
    [SerializeField] Color resetColor = new Color(1f, 0.5f, 0.5f);

    [Header("Audio")]
    [SerializeField] AudioSource audioSourceToFadeOut;

    Collider2D col;
    bool triggered;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;

        if (darknessController == null)
            darknessController = FindObjectOfType<DarknessController>();

        if (darknessController != null)
            darknessController.StartDarknessFade(fadeDuration, 1f, 1f);

        if (audioSourceToFadeOut != null)
            StartCoroutine(FadeOutAudio());

        StartCoroutine(ApplyAfterFade(other.GetComponent<Y2DMovementFinal>(), other.GetComponent<Y2DPlayerBehaviorWithHurt>()));
    }

    IEnumerator FadeOutAudio()
    {
        if (audioSourceToFadeOut == null) yield break;

        float startVolume = audioSourceToFadeOut.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            if (audioSourceToFadeOut == null) yield break;

            timer += Time.deltaTime;
            audioSourceToFadeOut.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        if (audioSourceToFadeOut != null)
            audioSourceToFadeOut.volume = 0f;
    }

    IEnumerator ApplyAfterFade(Y2DMovementFinal movement, Y2DPlayerBehaviorWithHurt playerBehavior)
    {
        yield return new WaitForSeconds(fadeDuration);

        if (movement != null)
        {
            movement.AddSpeed(-speedToRemove);
            movement.AddJumpForce(-jumpForceToRemove);
            movement.AddGravityScale(-gravityScaleToRemove);
        }

        if (objectToDestroy != null)
            Destroy(objectToDestroy);

        if (spawnTrigger != null && spawnTrigger.CurrentEnemy != null)
            Destroy(spawnTrigger.CurrentEnemy);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        if (resetPlayerHealth && playerBehavior != null)
        {
            playerBehavior.CurrentHealth = 2;
            playerBehavior.SetPermanentColor(resetColor);
        }

        Destroy(gameObject);
    }
}