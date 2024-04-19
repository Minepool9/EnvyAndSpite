using UnityEngine;

namespace DoomahLevelLoader.UnityComponents
{
	public class Blocker : AddressableReplacer
    {
        protected override void PostInstantiate(GameObject instantiatedObject)
        {
            BoxCollider originalCollider = GetComponent<BoxCollider>();
            if (originalCollider != null)
            {
                BoxCollider instantiatedCollider = instantiatedObject.AddComponent<BoxCollider>();
                instantiatedCollider.center = originalCollider.center;
                instantiatedCollider.size = new Vector3(originalCollider.size.z, originalCollider.size.y, originalCollider.size.x);
                
                Quaternion originalRotation = transform.rotation;

                Quaternion newRotation = Quaternion.Euler(0, originalRotation.eulerAngles.y + 90, 0);

                instantiatedObject.transform.rotation = newRotation;
				instantiatedObject.AddComponent<BlockerUpdater>();
				
                Destroy(originalCollider);
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

			Transform parent = transform.parent;
			if (parent != null)
			{
				foreach (Transform sibling in parent)
				{
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

			if (allEnemiesDead && parent != null)
			{
				foreach (Transform grandchild in parent)
				{
					if (grandchild.parent != null && grandchild.parent.GetComponent<WaveComponent>() != null)
					{
						continue;
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

			if (allEnemiesDead)
			{
				gameObject.SetActive(false);
			}
		}
    }
}
