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
            // Add a listener for the "load" button's click event
            load.onClick.AddListener(OnLoadButtonClick);
            
            // Add listeners for the "gofoward" and "goback" buttons
            gofowardinlist.onClick.AddListener(OnGoForwardButtonClick);
            gobackinlist.onClick.AddListener(OnGoBackButtonClick);
            
            // Set the initial text of levelname
            UpdateLevelName();
        }

        // Method to handle the "load" button click event
        private void OnLoadButtonClick()
        {
			Loaderscene.LoadScene();
        }

        // Method to handle the "gofoward" button click event
        private void OnGoForwardButtonClick()
        {
            // Call the MoveToNextFile method from the Loader class
            Loaderscene.NextBundle();
			UpdateLevelName();
        }

        // Method to handle the "goback" button click event
        private void OnGoBackButtonClick()
        {
            // Call the MoveToPreviousFile method from the Loader class
            Loaderscene.PreviousBundle();
			UpdateLevelName();
        }

		public void UpdateLevelName()
		{
			string fileName = Path.GetFileNameWithoutExtension(Loaderscene.scenePath);
			levelname.text = fileName;
		}
    }
}
