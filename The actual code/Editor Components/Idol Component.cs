using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace DoomahLevelLoader.UnityComponents
{
    public class IdolAssigner : MonoBehaviour
    {
        public AddressableReplacer Target;

        public bool oneTime = true;
        public bool moveToParent = true;
        public bool destroyThis = true;
		
        private void OnEnable()
        {
            Activate();
        }

        private bool _activated = false;
        public void Activate()
        {
            if (oneTime && _activated)
                return;

            _activated = true;

            GameObject targetObject = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Enemies/Idol.prefab").WaitForCompletion();
            if (targetObject == null)
            {
                Debug.LogWarning($"Tried to load asset, but it does not exist");
                enabled = false;
                return;
            }

            GameObject instantiatedObject = Instantiate(targetObject, transform.position, transform.rotation, transform);

            if (moveToParent)
                instantiatedObject.transform.SetParent(transform.parent, true);
			
			Idol idolComponent = instantiatedObject.GetComponent<Idol>();
			if (idolComponent != null && Target != null)
			{
				if (Target.eid != null)
				{
					idolComponent.overrideTarget = Target.eid;
				}
			}
			
            PostInstantiate(instantiatedObject);
			

            if (destroyThis)
            {
                Destroy(gameObject);
                gameObject.SetActive(false);
            }

            enabled = false;
        }

        protected virtual void PostInstantiate(GameObject instantiatedObject) { }
    }
}
