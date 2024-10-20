using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

/// <summary>Internal class.</summary>
[RequireComponent(typeof(Collider))]
public class ShowHoveredInfo : MonoBehaviour
{
	Mesh m_Mesh;
	static Material m_Material;
	bool m_IsHovering;

	void Start()
	{
		if (m_Material == null)
			m_Material = new Material(Shader.Find("Unlit/Color"));
		Mesh original = GetComponent<MeshFilter>().sharedMesh;
		m_Mesh = new Mesh();


		Vector3[] verts = new Vector3[]{Vector3.up, Vector3.right, Vector3.down, Vector3.left};
		int[] indicesForLineStrip = new int[]{0,1,2,3,0};
		m_Mesh.vertices = verts;
		m_Mesh.SetIndices(indicesForLineStrip, MeshTopology.LineStrip, 0);

		// m_Mesh.vertices = original.vertices;
		// int[] tris = original.triangles;
		// int[] lines = new int[tris.Length * 2];
		// int index = 0;
		// for (int i = 0; i < tris.Length; i += 3)
		// {
		// 	lines[index++] = tris[i];
		// 	lines[index++] = tris[i + 1];
		// 	lines[index++] = tris[i + 1];
		// 	lines[index++] = tris[i + 2];
		// 	lines[index++] = tris[i + 2];
		// 	lines[index++] = tris[i];
		// }

		// m_Mesh.SetIndices(lines, MeshTopology.Lines, 0, false);
	}

	void OnMouseEnter()
	{
		m_IsHovering = true;
	}

	void OnMouseExit()
	{
		m_IsHovering = false;
	}

	void OnGUI()
	{
		if (!m_IsHovering)
			return;

		GUI.Label(
			new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 600f, 900f),
			gameObject.name + "\n" + string.Join("\n",
				GetComponents<Component>().Select(x => x.GetType().ToString()).ToArray()));
	}

	void OnRenderObject()
	{
		Debug.Log("hello retard.");
		m_Material.SetPass(0);
		Graphics.DrawMeshNow(m_Mesh, transform.localToWorldMatrix, 0);
	}
}
