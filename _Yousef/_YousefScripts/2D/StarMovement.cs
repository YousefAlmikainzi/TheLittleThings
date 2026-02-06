using UnityEngine;
public class StarMovement : MonoBehaviour
{
    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float speed = 3f;
    private Vector3 startPosition;
    private float time;
    void Start()
    {
        startPosition = transform.position;
    }
    void Update()
    {
        time += Time.deltaTime * speed;
        float newY = startPosition.y + Mathf.Sin(time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}