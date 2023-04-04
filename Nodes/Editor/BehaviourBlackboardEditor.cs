using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraphs
{
	[CustomEditor(typeof(Blackboard))]
	public class BehaviourBlackboardEditor : Editor
	{
		private static readonly GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(150), GUILayout.ExpandWidth(true), GUILayout.MinHeight(18) };

		private static readonly Dictionary<string, string> basicTypesNames = new Dictionary<string, string>()
		{
			{"Bool", "Boolean" },
			{"Float", "Single"},
			{"Integer", "Int32"},
			{"String", "String"}
		};

		private Blackboard blackboard;

		private List<string> varsToRemove = new List<string>();

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			blackboard = target as Blackboard;
			foreach (var item in blackboard.variables)
			{
				ShowDataFieldGUI(item.Value);
			}
			if (GUILayout.Button("Add Variable"))
			{
				List<string> typesNames = new List<string>();
				var types = (typeof(Object)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Object))).ToList();
				for (int i = 0; i < types.Count; i++)
				{
					typesNames.Add(types[i].FullName.Replace(".","/"));
				}
				AddBasicTypes(typesNames);
				AddUnityStructs(typesNames);
				var mouseRect = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
				var searchProvider = ScriptableObject.CreateInstance<StringListSearchProvieder>();
				searchProvider.Initialize("Add Variable", typesNames.ToArray(), OnTypeSelected);
				SearchWindow.Open(new SearchWindowContext(mouseRect), searchProvider);
			}
			if (varsToRemove.Count > 0)
			{
				for (int i = 0; i < varsToRemove.Count; i++)
				{
					blackboard.RemoveVatiable(varsToRemove[i]);
				}
				varsToRemove.Clear();
			}
		}

		private void OnTypeSelected(string typeName)
		{
			var types = (typeof(Object)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Object))).ToList();
			var type = types.Find(x => x.Name == typeName);
			if (type != null)
			{
				AddNewVariable(type);
			}
			else
			{
				if (basicTypesNames.TryGetValue(typeName, out var basicTypeName))
				{
					var basicTypes = (typeof(object)).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(object))).ToList();
					type = basicTypes.Find(x => x.Name == basicTypeName);
				}
				if (type != null)
				{
					AddNewVariable(type);
				}
				else
				{
					if (typeName == typeof(Vector2).Name)
					{
						type = typeof(Vector2);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Vector2Int).Name)
					{
						type = typeof(Vector2Int);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Vector3).Name)
					{
						type = typeof(Vector3);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Vector3Int).Name)
					{
						type = typeof(Vector3Int);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Vector4).Name)
					{
						type = typeof(Vector4);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Quaternion).Name)
					{
						type = typeof(Quaternion);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Color).Name)
					{
						type = typeof(Color);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Rect).Name)
					{
						type = typeof(Rect);
						AddNewVariable(type);
						return;
					}

					if (typeName == typeof(Bounds).Name)
					{
						type = typeof(Bounds);
						AddNewVariable(type);
						return;
					}
				}
			}
		}

		void AddNewVariable(System.Type t)
		{
			Undo.RecordObject(blackboard, "Variable Added");
			var name = "my" + t.Name;
			while (blackboard.GetVariable(name) != null)
			{
				name += ".";
			}
			blackboard.AddVariable(name, t);
			EditorUtility.SetDirty(blackboard);
		}

		void ShowDataFieldGUI(Variable data)
		{
			var newVal = VariableField(data, blackboard, layoutOptions);
			if (!Equals(data.value, newVal))
			{
				Undo.RecordObject(blackboard, "Variable Value Change");
				data.value = newVal;
				EditorUtility.SetDirty(blackboard);
			}
		}
		
		object VariableField(Variable data, UnityEngine.Object contextParent, GUILayoutOption[] layoutOptions)
		{
			var o = data.value;
			var t = data.varType;
			///----------------------------------------------------------------------------------------------
			bool handled;
			EditorGUILayout.BeginHorizontal();
			data.name = EditorGUILayout.TextField(data.name);
			o = EditorUtils.DirectFieldControl(GUIContent.none, o, t, contextParent, null, out handled, layoutOptions);
			if (GUILayout.Button("X", GUILayout.Width(25)))
			{
				varsToRemove.Add(data.name);
			}
			EditorGUILayout.EndHorizontal();
			if (handled) { return o; }
			///----------------------------------------------------------------------------------------------

			////If some other type, show it in the generic object editor window
			//if (GUILayout.Button(string.Format("{0} {1}", t.Name, (o is IList) ? ((IList)o).Count.ToString() : string.Empty), layoutOptions))
			//{
			//    //we use bb.GetVariableByID to avoid undo creating new instance of variable and thus generic inspector, left inspecting something else
			//    GenericInspectorWindow.Show(data.name, t, contextParent, () => { return bb.GetVariableByID(data.ID).value; }, (newValue) => { bb.GetVariableByID(data.ID).value = newValue; });
			//}

			return o;
		}

		private void AddBasicTypes(List<string> types)
		{
			types.Add("Basic/Bool");
			types.Add("Basic/Float");
			types.Add("Basic/Integer");
			types.Add("Basic/String");
		}

		private void AddUnityStructs(List<string> types)
		{
			var structs = new List<string>();
			structs.Add($"UnityEngine/Structs/{nameof(Vector2)}");
			structs.Add($"UnityEngine/Structs/{nameof(Vector2Int)}");
			structs.Add($"UnityEngine/Structs/{nameof(Vector3)}");
			structs.Add($"UnityEngine/Structs/{nameof(Vector3Int)}");
			structs.Add($"UnityEngine/Structs/{nameof(Vector4)}");
			structs.Add($"UnityEngine/Structs/{nameof(Quaternion)}");
			structs.Add($"UnityEngine/Structs/{nameof(Color)}");
			structs.Add($"UnityEngine/Structs/{nameof(Rect)}");
			structs.Add($"UnityEngine/Structs/{nameof(Bounds)}");
			structs.Sort();
			types.AddRange(structs);
		}
	}

	public class GenericMenuBrowser : PopupWindowContent
	{
		public List<string> variables = new List<string>();

		public override Vector2 GetWindowSize()
		{
			return new Vector2(400, 150);
		}

		public GenericMenuBrowser(GenericMenu menu)
		{
			var menuType = typeof(GenericMenu);
			var properties = menuType.GetRuntimeProperties();
			var itemField = properties.First(x => x.Name == "menuItems");
			var value = itemField.GetValue(menu);
			var items = value as IList;
			variables = new List<string>();
			for (int i = 0; i < items.Count; i++)
			{
				variables.Add(items[i].ToString());
			}
		}

		public override void OnGUI(Rect rect)
		{
			for (int i = 0; i < variables.Count; i++)
			{
				GUILayout.Label(variables[i]);
			}
		}
	}
}