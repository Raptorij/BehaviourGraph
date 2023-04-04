using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
	public class NoteNode : Node
	{
		[SerializeField, TextArea(10, 20)]
		private string description;
		public string Description => description;

		public System.Action<NoteNode> DateChanged;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(NoteNode))]
	public class NoteNodeEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			//var noteNode = (target as NoteNode<>);
			//GUI.enabled = false;
			//EditorGUILayout.TextField("GUID", noteNode.guid);
			//GUI.enabled = true;
			//EditorGUI.BeginChangeCheck();
			//base.OnInspectorGUI();
			//if (EditorGUI.EndChangeCheck())
			//{
			//	noteNode.DateChanged?.Invoke(noteNode);
			//}
		}
	}
#endif
}