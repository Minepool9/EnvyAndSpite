using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

namespace DoomahLevelLoader
{
    public class LevelButtonScript : MonoBehaviour
    {
        private static LevelButtonScript instance;

        public Image LevelImageButtonThing;
		public Button LevelButtonReal;
		public TextMeshProUGUI NoLevel;
		public TextMeshProUGUI FileSize;
		public TextMeshProUGUI Author;
		public TextMeshProUGUI LevelName;
		

        public static LevelButtonScript Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LevelButtonScript>();

                    if (instance == null)
                    {
                        UnityEngine.Debug.LogError("LevelButtonScript instance not found in the scene.");
                    }
                }
                return instance;
            }
        }
    }
}
