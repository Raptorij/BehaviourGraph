using ParadoxNotion.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RaptorijDevelop.BehaviourGraph
{
	[Serializable]
	sealed public class SerializationPair
	{
		[TextArea(5, 5)]
		public string _json;
		public List<UnityEngine.Object> _references;
		public Type type;
		public SerializationPair() { _references = new List<UnityEngine.Object>(); }
	}

	public class Blackboard : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector] private string _serializedBlackboard;
		[SerializeField, HideInInspector] private List<UnityEngine.Object> _objectReferences;
		[SerializeField, HideInInspector] private SerializationPair[] _serializedVariables;

		[NonSerialized] private BlackboardSource _blackboard = new BlackboardSource();
		[NonSerialized] private bool haltForUndo = false;

		public Dictionary<string, Variable> variables { get { return _blackboard.variables; } set { _blackboard.variables = value; } }

		void ISerializationCallbackReceiver.OnBeforeSerialize() { SelfSerialize(); }
		void ISerializationCallbackReceiver.OnAfterDeserialize() { SelfDeserialize(); }

		private void SelfSerialize()
		{
			if (haltForUndo)
			{
				return;
			}

			var newReferences = new List<UnityEngine.Object>();
			var newSerialization = JSONSerializer.Serialize(typeof(BlackboardSource), _blackboard, newReferences);
			if (newSerialization != _serializedBlackboard || !newReferences.SequenceEqual(_objectReferences) || (_serializedVariables == null || _serializedVariables.Length != _blackboard.variables.Count))
			{

				haltForUndo = true;
				UndoUtility.RecordObject(this, UndoUtility.GetLastOperationNameOr("Blackboard Change"));
				haltForUndo = false;

				_serializedVariables = new SerializationPair[_blackboard.variables.Count];
				for (var i = 0; i < _blackboard.variables.Count; i++)
				{
					var serializedVariable = new SerializationPair();
					serializedVariable._json = JSONSerializer.Serialize(typeof(Variable), _blackboard.variables.ElementAt(i).Value, serializedVariable._references);
					_serializedVariables[i] = serializedVariable;
				}

				_serializedBlackboard = newSerialization;
				_objectReferences = newReferences;
			}
		}

		private void SelfDeserialize()
		{
			_blackboard = new BlackboardSource();
			if (!string.IsNullOrEmpty(_serializedBlackboard) /*&& ( _serializedVariables == null || _serializedVariables.Length == 0 )*/ )
			{
				JSONSerializer.TryDeserializeOverwrite<BlackboardSource>(_blackboard, _serializedBlackboard, _objectReferences);
			}

			//this is to handle prefab overrides
			if (_serializedVariables != null && _serializedVariables.Length > 0)
			{
				_blackboard.variables.Clear();
				for (var i = 0; i < _serializedVariables.Length; i++)
				{
					var variable = JSONSerializer.Deserialize<Variable>(_serializedVariables[i]._json, _serializedVariables[i]._references);
					_blackboard.variables[variable.name] = variable;
				}
			}
		}

		public void AddVariable(string varName, System.Type type)
		{
			if (variables.TryGetValue(varName, out var variable))
			{
				return;
			}

			var variableType = typeof(Variable<>).MakeGenericType(new Type[] { type });
			var newVariable = (Variable)Activator.CreateInstance(variableType);
			newVariable.name = varName;
			variables.Add(varName, newVariable);
		}

		public void RemoveVatiable(string varName)
		{
			if (variables.TryGetValue(varName, out var variable))
			{
				variables.Remove(varName);
			}
		}

		public Variable GetVariable(string varName)
		{
			variables.TryGetValue(varName, out var variable);
			return variable;
		}

		public bool TryGetVariable(string varName, out Variable variable)
		{
			variable = GetVariable(varName);
			return variable != null;
		}
	}

	[Serializable]
	public class VariableReference<T>
	{
		public string varName;
		public bool useConstValue;
		public T constValue;

		public bool TryGetVariable(Blackboard blackboard, out T blackboardVariable)
		{
			if (blackboard != null)
			{
				if (blackboard.TryGetVariable(varName, out var variable))
				{
					blackboardVariable = (T)variable.value;
					return true;
				}
			}

			if (useConstValue)
			{
				blackboardVariable = constValue;
				return true;
			}
			else
			{
				blackboardVariable = default(T);
				return false;
			}
		}

		public T GetVariable(Blackboard blackboard)
		{
			if (blackboard != null)
			{
				if (blackboard.TryGetVariable(varName, out var variable))
				{
					return (T)variable.value;
				}
			}
			return constValue;
		}

		internal bool TryGetVariable(object blackboard, out object perkAbility)
		{
			throw new NotImplementedException();
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(VariableReference<>))]
	public class IngredientDrawerUIE : PropertyDrawer
	{
		private bool foldout = true;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var varName = property.FindPropertyRelative("varName");
			var useConstValue = property.FindPropertyRelative("useConstValue");
			var constValue = property.FindPropertyRelative("constValue");
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, property.name + " (Variable Reference)");
			if (foldout)
			{
				EditorGUI.indentLevel++;
				GUI.enabled = !useConstValue.boolValue;
				var amountField = EditorGUILayout.PropertyField(varName);
				var rect = GUILayoutUtility.GetLastRect();
				rect.x += rect.width - 15;
				rect.width = 15;
				if (GUI.Button(rect, "+"))
				{

				}
				GUI.enabled = true;
				var unitField = EditorGUILayout.PropertyField(useConstValue);
				GUI.enabled = useConstValue.boolValue;
				var nameField = EditorGUILayout.PropertyField(constValue);
				GUI.enabled = true;
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
			EditorGUILayout.EndVertical();
		}
	}
#endif
}