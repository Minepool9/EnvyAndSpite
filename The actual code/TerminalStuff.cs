using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            // Call the InstantiatePrefab method directly from the Plugin class
            Plugin.Instance.InstantiatePrefab();
        }

        // Method to handle the "gofoward" button click event
        private void OnGoForwardButtonClick()
        {
            // Call the MoveToNextFile method from the Loader class
            Loader.MoveToNextFile();
			Plugin.Instance.firstTimeFlag = true;
        }

        // Method to handle the "goback" button click event
        private void OnGoBackButtonClick()
        {
            // Call the MoveToPreviousFile method from the Loader class
            Loader.MoveToPreviousFile();
			Plugin.Instance.firstTimeFlag = true;
        }

        // Method to update the text of levelname UI element with the current file name
        public void UpdateLevelName()
        {
            if (Loader.GetCurrentFileName() != null)
            {
                levelname.text = Loader.GetCurrentFileName();
            }
        }
    }
}
