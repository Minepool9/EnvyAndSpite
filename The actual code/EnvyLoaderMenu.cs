using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace DoomahLevelLoader
{
    public class EnvyLoaderMenu : MonoBehaviour
    {
        private static EnvyLoaderMenu instance;

        public GameObject ContentStuff;
        public Button MenuOpener;
        public GameObject LevelsMenu;
		public GameObject LevelsButton;
		public Button Goback;

        public static EnvyLoaderMenu Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EnvyLoaderMenu>();

                    if (instance == null)
                    {
                        Debug.LogError("EnvyLoaderMenu instance not found in the scene.");
                    }
                }
                return instance;
            }
        }
		
		private void Start()
        {
            MenuOpener.onClick.AddListener(OpenLevelsMenu);
			Goback.onClick.AddListener(GoBackToMenu);
        }

        private void OpenLevelsMenu()
        {
            LevelsMenu.SetActive(true);
            
            MenuOpener.gameObject.SetActive(false);
			MainMenuAgony.isAgonyOpen = true;
        }        
		
        private void GoBackToMenu()
        {
            LevelsMenu.SetActive(false);
            MenuOpener.gameObject.SetActive(true);
			MainMenuAgony.isAgonyOpen = false;
        }
    }
	
	public class DropdownHandler : MonoBehaviour
	{
		public Dropdown dropdown;

		public void OnDropdownValueChanged(int index)
		{
			int selectedDifficulty = index;

			MonoSingleton<PrefsManager>.Instance.SetInt("difficulty", selectedDifficulty);
		}
	}

}
