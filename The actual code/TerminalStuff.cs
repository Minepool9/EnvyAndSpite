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

            UpdateLevelName();

            Discord.onClick.AddListener(OnDiscordButtonClick);
        }

        private void OnLoadButtonClick()
        {
            Loaderscene.LoadScene();
        }

        private void OnGoForwardButtonClick()
        {
            Loaderscene.NextBundle();
            UpdateLevelName();
        }

        private void OnGoBackButtonClick()
        {
            Loaderscene.PreviousBundle();
            UpdateLevelName();
        }

        private void OnDiscordButtonClick()
        {
            Application.OpenURL("https://discord.gg/RY8J67neJ9");
        }

        public void UpdateLevelName()
        {
            string fileName = Path.GetFileNameWithoutExtension(Loaderscene.scenePath);
            levelname.text = fileName;
        }
    }
}
