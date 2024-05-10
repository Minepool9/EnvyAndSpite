using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
namespace DoomahLevelLoader
{
    public class RefreshAndDirectory : MonoBehaviour
    {
        private static RefreshAndDirectory instance;

        public Button Refresh;
		public Button Directory;

        public static RefreshAndDirectory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RefreshAndDirectory>();

                    if (instance == null)
                    {
                        UnityEngine.Debug.LogError("RefreshAndDirectory instance not found in the scene.");
                    }
                }
                return instance;
            }
        }
		
		private void Start()
        {
            Refresh.onClick.AddListener(Refreshaction);
            Directory.onClick.AddListener(DirectoryOpen);
        }
		
		private void Refreshaction()
		{
			_ = Loaderscene.Refresh();
		}
		
		private void DirectoryOpen()
		{
			Loaderscene.OpenFilesFolder();
		}
    }
}
