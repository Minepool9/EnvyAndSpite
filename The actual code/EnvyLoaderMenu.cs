using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

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
		public GameObject FuckingPleaseWait;

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
			CreateLevels();
        }
		
		public void CreateLevels()
		{
			for (int i = 0; i < Loaderscene.loadedAssetBundles.Count; i++)
			{
				GameObject buttonGO = Instantiate(LevelsButton, ContentStuff.transform);
				Button button = buttonGO.GetComponent<Button>();
				int index = i;
				button.onClick.AddListener(() =>
				{
					Loaderscene.currentAssetBundleIndex = index;
					Loaderscene.ExtractSceneName();
					Loaderscene.Loadscene();
				});

				LevelButtonScript levelButtonScript = buttonGO.GetComponent<LevelButtonScript>();

				string bundlePath = Loaderscene.bundleFolderPaths[index];

				Loaderscene.UpdateLevelPicture(levelButtonScript.LevelImageButtonThing,levelButtonScript.NoLevel,false,bundlePath);
				string Size = Loaderscene.GetAssetBundleSize(index);
				levelButtonScript.FileSize.text = Size;
			}
		}
		
		public void ClearContentStuffChildren()
		{
			foreach (Transform child in ContentStuff.transform)
			{
				Destroy(child.gameObject);
			}
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
		public TMP_Dropdown dropdown;

		private const string selectedDifficultyKey = "difficulty";
		private int savedDifficulty = MonoSingleton<PrefsManager>.Instance.GetInt(selectedDifficultyKey, 2);

		private void Start()
		{
			MonoSingleton<PrefsManager>.Instance.SetInt(selectedDifficultyKey, 2);
			dropdown.value = savedDifficulty;
			
			dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
		}

		public void OnDropdownValueChanged(int index)
		{
			MonoSingleton<PrefsManager>.Instance.SetInt(selectedDifficultyKey, index);
		}
	}

}
