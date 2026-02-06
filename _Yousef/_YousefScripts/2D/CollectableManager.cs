using UnityEngine;
using TMPro;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectableText;
    [SerializeField] private int totalCollectables = 10;
    [SerializeField] private GameObject objectToDestroy;

    private void Start()
    {
        CollectablesScript.SetupUI(collectableText, totalCollectables);
        CollectablesScript.OnAllCollected += HandleAllCollected;
    }

    private void OnDestroy()
    {
        CollectablesScript.OnAllCollected -= HandleAllCollected;
    }

    private void HandleAllCollected()
    {
        if (objectToDestroy != null)
            Destroy(objectToDestroy);
    }
}
