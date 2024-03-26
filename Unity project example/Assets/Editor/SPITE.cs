using UnityEngine;
using UnityEditor;
using DoomahLevelLoader;

public class SpawnerEditor : EditorWindow
{
    private string[] enemyOptions = { "Filth", "Virtue", "Cerberus", "Drone", "Maurice", "Stray", "Schism", "Soldier", "SwordsMachine", "Sisyphus_Insurrectionist", "Hideous_Mass", "StreetCleaner", "Stalker", "Ferryman", "Mannequin", "Mindflayer", "Sentry", "V2", "Gabe_First", "Gabe_Second", "GutterTank", "GutterMan", "Puppet"};
    private string[] miscOptions = { "TriggerZone", "Wave", "Blocker", "Health_Power_Up", "Duel_Power_Up", "Green_Grapple", "Blue_Grapple", "PlayerStart", "Yellow_Shop"};
	private int selectedEnemyIndex = 0;
	private int selectedMiscIndex = 0;

    [MenuItem("SPITE/Spawner window", false, 10)]
    static void Init()
    {
        SpawnerEditor window = (SpawnerEditor)EditorWindow.GetWindow(typeof(SpawnerEditor));
        window.Show();
    }

	void OnGUI()
	{
		GUILayout.Label("SPITE", EditorStyles.boldLabel);

		GUILayout.Space(10);

		GUILayout.Label("Enemies", EditorStyles.boldLabel);
		selectedEnemyIndex = EditorGUILayout.Popup(selectedEnemyIndex, enemyOptions);
		if (GUILayout.Button("Spawn Enemy"))
		{
			SpawnObject("Enemies", enemyOptions[selectedEnemyIndex]);
		}

		GUILayout.Space(10);

		GUILayout.Label("Misc", EditorStyles.boldLabel);
		selectedMiscIndex = EditorGUILayout.Popup(selectedMiscIndex, miscOptions);
		if (GUILayout.Button("Spawn Misc"))
		{
			SpawnObject("Misc", miscOptions[selectedMiscIndex]);
		}
	}

	void SpawnObject(string category, string prefabName)
	{
		GameObject newObj = new GameObject("Spawnable_" + prefabName);
		
		if (prefabName == "TriggerZone")
		{
			BoxCollider collider = newObj.AddComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = new Vector3(5f, 5f, 5f);
		}
		else if (prefabName == "Blocker")
		{
			BoxCollider collider = newObj.AddComponent<BoxCollider>();
			collider.size = new Vector3(5f, 5f, 1f);
		}
		else if (prefabName == "CheckPoint")
		{
			BoxCollider collider = newObj.AddComponent<BoxCollider>();
			collider.size = new Vector3(10f, 2f, 0.1f); // Set size for CheckPoint
			
			// Attach ReallyCustomRoomManager script only to CheckPoint
			ReallyCustomRoomManager customRoomManager = newObj.AddComponent<ReallyCustomRoomManager>();
		}
		
		Undo.RegisterCreatedObjectUndo(newObj, "Spawn " + prefabName);

		Debug.Log("Spawned " + category + ": " + prefabName);
	}

}
