using UnityEngine;
using System.Collections;

public class DarknessController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform lightCircle;
    [SerializeField] Material darknessMaterial;
    [SerializeField] SpriteRenderer lightCircleRenderer;

    [SerializeField] float lightRadius = 2f;
    [SerializeField, Range(0f, 1f)] float edgeSoftness = 0.0f;
    [SerializeField] Color darknessColor = Color.black;
    [SerializeField] float followSpeed = 10f;

    Vector3 velocity;
    Coroutine runningFade;

    void Start()
    {
        if (darknessMaterial == null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) darknessMaterial = sr.material;
        }
        else
        {
            darknessMaterial = new Material(darknessMaterial);
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.material = darknessMaterial;
        }

        if (darknessMaterial != null && darknessMaterial.HasProperty("_DarknessColor"))
            darknessMaterial.SetColor("_DarknessColor", darknessColor);

        if (lightCircle != null && lightCircleRenderer == null)
            lightCircleRenderer = lightCircle.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (player != null)
            transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, 1f / Mathf.Max(0.0001f, followSpeed));

        if (darknessMaterial == null || lightCircle == null) return;

        darknessMaterial.SetVector("_LightPosition", new Vector4(lightCircle.position.x, lightCircle.position.y, lightCircle.position.z, 0f));
        darknessMaterial.SetFloat("_LightRadius", lightRadius);
        darknessMaterial.SetFloat("_EdgeSoftness", edgeSoftness);
    }

    public void StartDarknessFade(float darknessDuration, float circleDuration, float targetDarknessAlpha = 1f, float targetCircleAlpha = 1f)
    {
        if (runningFade != null) StopCoroutine(runningFade);
        runningFade = StartCoroutine(FadeCoroutine(darknessDuration, circleDuration, targetDarknessAlpha, targetCircleAlpha));
    }

    // Overload for backward compatibility (single duration for both)
    public void StartDarknessFade(float duration, float targetDarknessAlpha = 1f, float targetCircleAlpha = 1f)
    {
        StartDarknessFade(duration, duration, targetDarknessAlpha, targetCircleAlpha);
    }

    IEnumerator FadeCoroutine(float darknessDuration, float circleDuration, float targetDarknessAlpha, float targetCircleAlpha)
    {
        float elapsed = 0f;
        float startDarknessA = 0f;
        float startCircleA = 0f;

        if (darknessMaterial != null && darknessMaterial.HasProperty("_DarknessColor"))
            startDarknessA = darknessMaterial.GetColor("_DarknessColor").a;

        if (lightCircleRenderer != null)
            startCircleA = lightCircleRenderer.color.a;

        darknessDuration = Mathf.Max(0.0001f, darknessDuration);
        circleDuration = Mathf.Max(0.0001f, circleDuration);

        float maxDuration = Mathf.Max(darknessDuration, circleDuration);
        bool darknessComplete = false;
        bool circleComplete = false;

        while (elapsed < maxDuration)
        {
            // Fade darkness
            if (!darknessComplete && darknessMaterial != null && darknessMaterial.HasProperty("_DarknessColor"))
            {
                float darknessT = Mathf.Clamp01(elapsed / darknessDuration);
                Color c = darknessMaterial.GetColor("_DarknessColor");
                c.a = Mathf.Lerp(startDarknessA, targetDarknessAlpha, darknessT);
                darknessMaterial.SetColor("_DarknessColor", c);

                if (darknessT >= 1f) darknessComplete = true;
            }

            // Fade light circle
            if (!circleComplete && lightCircleRenderer != null)
            {
                float circleT = Mathf.Clamp01(elapsed / circleDuration);
                Color lc = lightCircleRenderer.color;
                lc.a = Mathf.Lerp(startCircleA, targetCircleAlpha, circleT);
                lightCircleRenderer.color = lc;

                if (circleT >= 1f) circleComplete = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (darknessMaterial != null && darknessMaterial.HasProperty("_DarknessColor"))
        {
            Color c = darknessMaterial.GetColor("_DarknessColor");
            c.a = targetDarknessAlpha;
            darknessMaterial.SetColor("_DarknessColor", c);
        }

        if (lightCircleRenderer != null)
        {
            Color lc = lightCircleRenderer.color;
            lc.a = targetCircleAlpha;
            lightCircleRenderer.color = lc;
        }

        runningFade = null;
    }

    public void SetLightRadius(float radius)
    {
        lightRadius = radius;
    }
}