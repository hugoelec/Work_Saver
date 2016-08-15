using UnityEngine;
using System.Collections;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class input : MonoBehaviour {

	public string importFilePath = "Enter the path";

	private GameObject emptyPrefabWithMeshRenderer;
	public GameObject spawnedPrefab;
	private Mesh importedMesh;

	private ObjImporter objImporter;
	// Use this for initialization

	private bool run = false;
	
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (run)
				if (!objImporter._mesh)
					print (objImporter._run ());
				else
					run = false;
		
	}
	void OnGUI ()
	{
		if (GUILayout.Button ("Load", GUILayout.Width(80)))
		{
			#if UNITY_EDITOR
			importFilePath = EditorUtility.OpenFilePanel ("" , importFilePath , "*.*");
			#endif
			
			if (importFilePath.Length != 0)
			{
				objImporter = new ObjImporter();
			//	spawnedPrefab.GetComponent<MeshFilter>().mesh = objImporter.ImportFile(importFilePath);//importedMesh;

				objImporter.ImportFile(importFilePath);
				run = true;
			}
		}
	}
}
