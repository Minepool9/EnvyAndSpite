using UnityEngine;

namespace DoomahLevelLoader.UnityComponents
{
public class PlayerStart : MonoBehaviour
{
    // Start
    void Awake()
    {
        // Check if the the transform is not null
        if (transform != null)
        {
            // Assign the position of the NewMovement instance to the position of the current transform
            NewMovement.Instance.transform.position = transform.position;
			Destroy(gameObject);
        }
        else
        {
            Debug.LogError("NewMovement script instance or transform is null.");
        }
    }
}
}