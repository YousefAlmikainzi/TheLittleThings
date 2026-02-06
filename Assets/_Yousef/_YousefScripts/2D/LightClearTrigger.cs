using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightClearTrigger : MonoBehaviour
{
    [SerializeField] DarknessController darknessController;
    [SerializeField] float darknessFadeDuration = 2f;
    [SerializeField] float lightCircleFadeDuration = 1.5f;
    [SerializeField] bool triggerOnce = true;
    [SerializeField] string playerTag = "Player";
    [SerializeField] GameObject gameObjectToEnable;
    [SerializeField] bool destroyAfterTrigger = true;

    [Header("Player Color Settings")]
    [SerializeField] Color newDefaultColor = new Color(1f, 0.5f, 0.5f);

    [Header("Audio")]
    [SerializeField] AudioSource audioSourceToEnable;

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (darknessController == null)
            darknessController = FindObjectOfType<DarknessController>();

        if (darknessController != null)
            darknessController.StartDarknessFade(darknessFadeDuration, lightCircleFadeDuration, 0f, 0f);

        if (gameObjectToEnable != null)
            gameObjectToEnable.SetActive(true);

        if (audioSourceToEnable != null)
            audioSourceToEnable.enabled = true;

        var player = other.GetComponent<Y2DPlayerBehaviorWithHurt>();
        if (player != null)
        {
            player.SetPermanentColor(newDefaultColor);
            player.SetOnlyEvilEnemyDamage(true);
        }

        if (triggerOnce)
        {
            if (col != null) col.enabled = false;
            enabled = false;
        }

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}