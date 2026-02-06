using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class WinningStar : MonoBehaviour
{
    [SerializeField] AudioClip collectSound;
    [SerializeField] string playerTag = "Player";

    [Header("Float Motion")]
    [SerializeField] float floatAmplitude = 0.2f;
    [SerializeField] float floatFrequency = 2f;

    [Header("External Fade Target")]
    [SerializeField] GameObject fadeObject;
    [SerializeField] SpriteRenderer fadeSprite;

    [Header("Fade Settings")]
    [SerializeField] float fadeDelay = 20f;
    [SerializeField] float fadeDuration = 2f;

    [SerializeField] int nextSceneIndex = 2;

    bool collected;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (fadeSprite != null)
        {
            Color c = fadeSprite.color;
            c.a = 0f;
            fadeSprite.color = c;
        }
    }

    void Update()
    {
        if (!collected)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag(playerTag)) return;

        collected = true;

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        var movement = other.GetComponent<Y2DMovementFinal>();
        if (movement) movement.enabled = false;

        var rb = other.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        var anim = other.GetComponent<Animator>();
        if (anim)
            anim.SetTrigger("hasWon");

        var sr = other.GetComponent<SpriteRenderer>();
        if (sr)
        {
            bool lockedFlip = sr.flipX;
            sr.flipX = lockedFlip;
        }

        if (fadeObject != null)
            fadeObject.SetActive(false);

        if (fadeSprite != null)
        {
            Color c = fadeSprite.color;
            c.a = 0f;
            fadeSprite.color = c;
            StartCoroutine(FadeSprite());
        }
    }

    IEnumerator FadeSprite()
    {
        yield return new WaitForSeconds(fadeDelay);

        float timer = 0f;
        Color c = fadeSprite.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            fadeSprite.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeSprite.color = c;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
