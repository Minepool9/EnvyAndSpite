using UnityEngine;

namespace DoomahLevelLoader.UnityComponents
{
	public class Blocker : AddressableReplacer
    {
        protected override void PostInstantiate(GameObject instantiatedObject)
        {
            // Copy BoxCollider from the original object to the instantiated object
            BoxCollider originalCollider = GetComponent<BoxCollider>();
            if (originalCollider != null)
            {
                BoxCollider instantiatedCollider = instantiatedObject.AddComponent<BoxCollider>();
                instantiatedCollider.center = originalCollider.center;
                instantiatedCollider.size = new Vector3(originalCollider.size.z, originalCollider.size.y, originalCollider.size.x);
                
                // Get the rotation of the original object
                Quaternion originalRotation = transform.rotation;

                // Add 90 degrees to the Y-axis rotation
                Quaternion newRotation = Quaternion.Euler(0, originalRotation.eulerAngles.y + 90, 0);

                // Apply the new rotation to the instantiated object
                instantiatedObject.transform.rotation = newRotation;
				instantiatedObject.AddComponent<BlockerUpdater>();
            }
            else
            {
                Debug.LogWarning("Original object does not have a BoxCollider component.");
            }
        }
	}
	
    public class BlockerUpdater : MonoBehaviour
    {
        private float timer = 0f;
        private float checkInterval = 0.2f;

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                CheckEnemies();
            }
        }
		
		void CheckEnemies()
		{
			bool allEnemiesDead = true;

			// Check siblings
			Transform parent = transform.parent;
			if (parent != null)
			{
				foreach (Transform sibling in parent)
				{
					// Skip processing if the sibling has WaveComponent script
					if (sibling.GetComponent<WaveComponent>() != null)
						continue;

					EnemyIdentifier enemyIdentifier = sibling.GetComponent<EnemyIdentifier>();
					if (enemyIdentifier != null && !enemyIdentifier.dead)
					{
						allEnemiesDead = false;
						break;
					}
				}
			}

			// Check parent's grandchildren
			if (allEnemiesDead && parent != null)
			{
				foreach (Transform grandchild in parent)
				{
					// Check if the parent of the grandchildren has a WaveComponent
					if (grandchild.parent != null && grandchild.parent.GetComponent<WaveComponent>() != null)
					{
						continue; // Skip processing if parent has WaveComponent
					}

					foreach (Transform child in grandchild)
					{
						EnemyIdentifier enemyIdentifier = child.GetComponent<EnemyIdentifier>();
						if (enemyIdentifier != null && !enemyIdentifier.dead)
						{
							allEnemiesDead = false;
							break;
						}
					}
					if (!allEnemiesDead)
						break;
				}
			}

			// Deactivate the instantiated object if all enemies are dead
			if (allEnemiesDead)
			{
				gameObject.SetActive(false);
			}
		}
    }
}
