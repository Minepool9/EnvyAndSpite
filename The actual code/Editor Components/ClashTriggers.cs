using UnityEngine;
using System;

namespace DoomahLevelLoader.UnityComponents
{
	public class ClashTriggerDisable : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.tag == "Player";
			if (flag)
			{
				MonoSingleton<PlayerTracker>.Instance.ChangeToFPS();
			}
		}
	}
	
	public class ClashTriggerEnable : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.tag == "Player" && !this.hasenabled;
			if (flag)
			{
				MonoSingleton<PlayerTracker>.Instance.ChangeToPlatformer();
				bool onlyOnce = this.OnlyOnce;
				if (onlyOnce)
				{
					this.hasenabled = true;
				}
			}
		}

		private bool hasenabled = false;

		public bool OnlyOnce = false;
	}
}
