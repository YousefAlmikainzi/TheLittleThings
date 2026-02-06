using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PaintingTransition : MonoBehaviour
{
    [SerializeField] SpriteRenderer transitionSprite;
    [SerializeField] float fadeDuration = 1.5f;
    [SerializeField] AudioClip transitionSound;

    bool playerInside = false;
    bool used = false;

    Y2DMovementFinalNoJump playerMovement;
    Rigidbody2D playerRb;       // <--- added
    Animator playerAnim;        // <--- added

    void Start()
    {
        if (transitionSprite == null)
            Debug.LogWarning("transitionSprite not assigned on PaintingTransition.");

        // Ensure sprite starts transparent
        Color c = transitionSprite.color;
        c.a = 0f;
        transitionSprite.color = c;
    }

    void Update()
    {
        if (!playerInside || used)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            used = true;

            // Stop physics motion and lock the player in place
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                // Freeze position & rotation so physics won't move the player anymore
                playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // Force idle animation (movement script normally sets "Speed")
            if (playerAnim != null)
            {
                playerAnim.SetFloat("Speed", 0f);
            }

            // Disable movement script last so it cannot override the above
            if (playerMovement != null)
                playerMovement.enabled = false;

            if (transitionSound != null)
                AudioSource.PlayClipAtPoint(transitionSound, Camera.main.transform.position);

            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        float t = 0f;
        Color c = transitionSprite.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            transitionSprite.color = c;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerMovement = other.GetComponent<Y2DMovementFinalNoJump>();
            playerRb = other.GetComponent<Rigidbody2D>();   // cache the rb
            playerAnim = other.GetComponent<Animator>();    // cache the animator
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // only clear if transition wasn't used yet
            if (!used)
            {
                playerInside = false;
                playerMovement = null;
                playerRb = null;
                playerAnim = null;
            }
        }
    }
}
