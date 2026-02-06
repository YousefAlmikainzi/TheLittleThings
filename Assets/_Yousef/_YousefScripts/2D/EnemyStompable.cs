using UnityEngine;
using System.Collections;

public class EnemyStompable : MonoBehaviour
{
    [SerializeField] float squashAmount = 0.2f;
    [SerializeField] float squashDuration = 0.1f;

    Vector3 originalLocalPosition;
    bool isSquashing;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void OnStomped()
    {
        if (!isSquashing)
            StartCoroutine(SquashRoutine());
    }

    System.Collections.IEnumerator SquashRoutine()
    {
        isSquashing = true;

        Vector3 downPos = originalLocalPosition + Vector3.down * squashAmount;
        float timer = 0f;

        while (timer < squashDuration)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPosition, downPos, timer / squashDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = downPos;

        timer = 0f;
        while (timer < squashDuration)
        {
            transform.localPosition = Vector3.Lerp(downPos, originalLocalPosition, timer / squashDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        isSquashing = false;
    }
}