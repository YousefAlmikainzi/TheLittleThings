using UnityEngine;
using System.Collections;

public class EnemyStompableMoving : MonoBehaviour
{
    [SerializeField] float squashAmount = 0.2f;
    [SerializeField] float squashDuration = 0.1f;

    float originalYPosition;
    bool isSquashing;

    void Start()
    {
        originalYPosition = transform.position.y;
    }

    void OnStomped()
    {
        if (!isSquashing)
            StartCoroutine(SquashRoutine());
    }

    System.Collections.IEnumerator SquashRoutine()
    {
        isSquashing = true;

        float downY = originalYPosition - squashAmount;
        float timer = 0f;

        while (timer < squashDuration)
        {
            float newY = Mathf.Lerp(originalYPosition, downY, timer / squashDuration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < squashDuration)
        {
            float newY = Mathf.Lerp(downY, originalYPosition, timer / squashDuration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);
        isSquashing = false;
    }
}