using Unity.Cinemachine;
using UnityEngine;

public class PixelPerfectFix : MonoBehaviour
{
    [SerializeField] private int pixelsPerUnit = 16;

    private CinemachineBrain brain;

    void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        float gridSize = 1f / pixelsPerUnit;

        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;

        transform.position = pos;
    }
}