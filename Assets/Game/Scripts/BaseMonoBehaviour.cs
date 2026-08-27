using UnityEngine;

public abstract class BaseMonoBehaviour : MonoBehaviour
{
    protected bool ValidateReference<T>(T reference, string fieldName) where T : class
    {
        if (reference == null)
        {
            Debug.LogWarning($"[{gameObject.name}] {fieldName} is null!");
            return false;
        }

        return true;
    }
}