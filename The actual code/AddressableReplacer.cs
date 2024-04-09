using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DoomahLevelLoader.UnityComponents
{
    public class AddressableReplacer : MonoBehaviour
    {
        public string targetAddress;

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

            GameObject targetObject = Addressables.LoadAssetAsync<GameObject>(targetAddress).WaitForCompletion();
            if (targetObject == null)
            {
                Debug.LogWarning($"Tried to load asset at address {targetAddress}, but it does not exist");
                enabled = false;
                return;
            }

            GameObject instantiatedObject = Instantiate(targetObject, transform.position, transform.rotation, transform);

            if (moveToParent)
                instantiatedObject.transform.SetParent(transform.parent, true);

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