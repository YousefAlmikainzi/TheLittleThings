using UnityEngine;
using System.Collections;

public class ElevatorStompSquash : MonoBehaviour
{
    [SerializeField] float squashAmount = 0.2f;
    [SerializeField] float squashDuration = 0.1f;

    bool isSquashing;

    void OnStomped()
    {
        if (!isSquashing)
            StartCoroutine(SquashDown());
    }

    IEnumerator SquashDown()
    {
        isSquashing = true;

        float startY = transform.position.y;
        float targetY = startY - squashAmount;
        float timer = 0f;

        while (timer < squashDuration)
        {
            float newY = Mathf.Lerp(startY, targetY, timer / squashDuration);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}
