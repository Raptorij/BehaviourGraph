using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;

public class TacticGraphClassTemplates
{
    [MenuItem("Assets/Create/Behaviour Graph/C# Behaviour Node Template")]
    static void CreateTacticTemplate()
    {
        TextAsset tacticTemplate = Resources.Load<TextAsset>("ScriptTemplates/BehaviourNodeTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(tacticTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewBehaviourNode.cs");
    }

    [MenuItem("Assets/Create/Behaviour Graph/C# Node Ability Template")]
    static void CreateNodeBehaviourTemplate()
    {
        TextAsset nodeBehaviourTemplate = Resources.Load<TextAsset>("ScriptTemplates/NodeAbilityTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(nodeBehaviourTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewNodeAbility.cs");
    }

    [MenuItem("Assets/Create/Behaviour Graph/C# Condition Template")]
    static void CreateConditionTemplate()
    {
        TextAsset conditionTemplate = Resources.Load<TextAsset>("ScriptTemplates/ConditionTemplate.cs");
        var assetPath = AssetDatabase.GetAssetPath(conditionTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(assetPath, "NewNewCondition.cs");
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
