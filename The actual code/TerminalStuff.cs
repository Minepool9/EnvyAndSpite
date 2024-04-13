using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace DoomahLevelLoader
{
    public class EnvyandSpiteterimal : MonoBehaviour
    {
        // Singleton instance
        private static EnvyandSpiteterimal instance;

        // Reference to UI elements
        public Text levelname;
        public Text loadbutton;
        public Button load;
        public Button gofowardinlist;
        public Button gobackinlist;

        // Static property to access the singleton instance
        public static EnvyandSpiteterimal Instance
        {
            get
            {
                // If the instance doesn't exist, find it in the scene
                if (instance == null)
                {
                    instance = FindObjectOfType<EnvyandSpiteterimal>();

                    // If it still doesn't exist, log an error
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
