using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace DoomahLevelLoader.UnityComponents
{
    public class AddressableReplacer : MonoBehaviour
    {
        public string targetAddress;
        public bool oneTime = true;
        public bool moveToParent = true;
        public bool destroyThis = true;
        public bool IsBoss = false;
        public string BossName;
        public bool IsSanded = false;
        public bool IsPuppet = false;
        public bool IsRadient = false;
        public float RadienceTier;
        public float DamageTier;
        public float SpeedTier;
        public float HealthTier;

        internal EnemyIdentifier eid;

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

            eid = instantiatedObject.GetComponent<EnemyIdentifier>();

            // If eid is still null, try getting the component from the first child
            if (eid == null && instantiatedObject.transform.childCount > 0)
            {
                eid = instantiatedObject.transform.GetChild(0).GetComponent<EnemyIdentifier>();
            }

            if (moveToParent)
                instantiatedObject.transform.SetParent(transform.parent, true);

            PostInstantiate(instantiatedObject);

            if (eid != null && IsBoss)
            {
                BossHealthBar bossHealthBar = eid.gameObject.AddComponent<BossHealthBar>();
                if (!string.IsNullOrEmpty(BossName))
                {
                    bossHealthBar.bossName = BossName;
                }
            }

            if (eid != null && IsSanded)
                eid.Sandify(false);

            if (eid != null && IsPuppet)
            {
                eid.PuppetSpawn();
                eid.puppet = true;
            }

            if (eid != null && IsRadient)
            {
                eid.radianceTier = RadienceTier;
                eid.healthBuffModifier = HealthTier;
                eid.speedBuffModifier = SpeedTier;
                eid.damageBuffModifier = DamageTier;
                eid.BuffAll();
            }

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
