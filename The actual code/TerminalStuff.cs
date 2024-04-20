using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace DoomahLevelLoader
{
    public class EnvyandSpiteterimal : MonoBehaviour
    {
        private static EnvyandSpiteterimal instance;

        public Text levelname;
        public Text loadbutton;
        public Button load;
        public Button gofowardinlist;
        public Button gobackinlist;
        public Image Levelpicture;
        public Text FrownyFace;
        public Button Discord;

        public static EnvyandSpiteterimal Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EnvyandSpiteterimal>();

                    if (instance == null)
                    {
                        Debug.LogError("EnvyandSpiteterimal instance not found in the scene.");
                    }
                }
                return instance;
            }
        }

        private void Start()
        {
            load.onClick.AddListener(OnLoadButtonClick);

            gofowardinlist.onClick.AddListener(OnGoForwardButtonClick);
            gobackinlist.onClick.AddListener(OnGoBackButtonClick);
			
			Levelpicture.color = Color.white;
			
            UpdateLevelName();
			UpdateLevelPicture();

            Discord.onClick.AddListener(OnDiscordButtonClick);
        }
		
		
	    private void UpdateLevelPicture()
		{
			string bundleFolderPath = Loaderscene.GetCurrentBundleFolderPath();

			if (!string.IsNullOrEmpty(bundleFolderPath))
			{
				string[] imageFiles = Directory.GetFiles(bundleFolderPath, "*.png");
				if (imageFiles.Length > 0)
				{
					string imagePath = imageFiles[0];
					Texture2D tex = LoadTextureFromFile(imagePath);
					if (tex != null)
					{	
						tex.filterMode = FilterMode.Point;
						Levelpicture.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
					}
				}
			}
		}
		
		private Texture2D LoadTextureFromFile(string path)
		{
			byte[] fileData = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(fileData);
			return texture;
		}
		
        private void OnLoadButtonClick()
        {
            Loaderscene.Loadscene();
        }

        private void OnGoForwardButtonClick()
        {
            Loaderscene.MoveToNextAssetBundle();
			Loaderscene.ExtractSceneName();
            UpdateLevelName();
			UpdateLevelPicture();
        }

        private void OnGoBackButtonClick()
        {
            Loaderscene.MoveToPreviousAssetBundle();
			Loaderscene.ExtractSceneName();
            UpdateLevelName();
			UpdateLevelPicture();
        }

        private void OnDiscordButtonClick()
        {
            Application.OpenURL("https://discord.gg/RY8J67neJ9");
        }

        public void UpdateLevelName()
        {
            levelname.text = Loaderscene.LoadedSceneName;
        }
    }
}
