using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;

public class TacticGraphClassTemplates
{
    [MenuItem("Assets/Create/Tactic Graph/C# Tactic Template")]
    static void CreateTacticTemplate()
    {
        TextAsset tacticTemplate = Resources.Load<TextAsset>("ScriptTemplates/TacticTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(tacticTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewTactic.cs");
    }

    [MenuItem("Assets/Create/Tactic Graph/C# Node Behaviour Template")]
    static void CreateNodeBehaviourTemplate()
    {
        TextAsset nodeBehaviourTemplate = Resources.Load<TextAsset>("ScriptTemplates/NodeBehaviourTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(nodeBehaviourTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewNodeBehaviour.cs");
    }

    [MenuItem("Assets/Create/Tactic Graph/C# Condition Template")]
    static void CreateConditionTemplate()
    {
        TextAsset conditionTemplate = Resources.Load<TextAsset>("ScriptTemplates/ConditionTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(conditionTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewCondition.cs");
    }

    static string GetCurrentFolderPath()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();
        Debug.Log(pathToCurrentFolder);
        return pathToCurrentFolder;
    }
}
