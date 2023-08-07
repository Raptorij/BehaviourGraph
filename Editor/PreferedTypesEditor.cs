using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace RaptorijDevelop.BehaviourGraph
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PreferedTypes))]
	public class PreferedTypesEditor : Editor
	{
        private PreferedTypes pt;

        private List<string> typeNames = new List<string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            pt = target as PreferedTypes;
            if (GUILayout.Button("Add New Prefered Type"))
			{
                List<Type> allTypes = GetAllTypesInAssembly(new string[] { "" });
                typeNames = allTypes.Select(x => x.FullName.Replace(".", "/")).ToList();
                typeNames.RemoveAll(y => pt.types.Contains(y));
                var mouseRect = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

				var searchProvider = ScriptableObject.CreateInstance<StringListSearchProvieder>();
				searchProvider.Initialize("Types", typeNames.ToArray(), OnTypeSelected);
				SearchWindow.Open(new SearchWindowContext(mouseRect), searchProvider);
			}
        }

        private void OnTypeSelected(string obj)
        {
            if (!pt.types.Contains(obj))
            {
                var typeName = typeNames.Find(x => x.Contains(obj));
                pt.types.Add(typeName);
            }
        }

        public static List<Type> GetAllTypesInAssembly(string[] pAssemblyNames)
        {
            List<Type> results = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (string assemblyName in pAssemblyNames)
                {
                    if (assembly.FullName.StartsWith(assemblyName))
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            results.Add(type);
                        }
                        break;
                    }
                }
            }
            return results;
        }
    }
#endif
}