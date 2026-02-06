using UnityEngine;

public class StompableRelay : MonoBehaviour
{
    void OnStomped()
    {
        transform.parent.SendMessage("OnStomped", SendMessageOptions.DontRequireReceiver);
    }
}