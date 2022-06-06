#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using ZB.Gameplay;
using ZB.Gameplay.PVP;

public static class ScriptableObjectCreator
{
    #region Class Methods

    [MenuItem("Zodi Bomb/Create/Scriptable Objects/AI/AI Group States")]
    public static void CreateAIGroupStatesScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(AIGroupStatesConfig));
        ShowEditorWindow("Create an AI group of states scriptable object", scriptableObjectTypes);
    }

    [MenuItem("Zodi Bomb/Create/Scriptable Objects/AI/AI State")]
    public static void CreateAIStateScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(AIStateConfig));
        ShowEditorWindow("Create an AI state scriptable object", scriptableObjectTypes);
    }

    [MenuItem("Zodi Bomb/Create/Scriptable Objects/AI/AI Action")]
    public static void CreateAIActionScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(AIActionConfig));
        ShowEditorWindow("Create an AI action scriptable object", scriptableObjectTypes);
    }

    [MenuItem("Zodi Bomb/Create/Scriptable Objects/AI/AI Transition")]
    public static void CreateAITransitionScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(AITransitionConfig));
        ShowEditorWindow("Create an AI transition scriptable object", scriptableObjectTypes);
    }

    [MenuItem("Zodi Bomb/Create/Scriptable Objects/AI/AI Decision")]
    public static void CreateAIDecisionScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(AIDecisionConfig));
        ShowEditorWindow("Create an AI decision scriptable object", scriptableObjectTypes);
    }


    [MenuItem("Zodi Bomb/Create/Scriptable Objects/Item Model")]
    public static void CreateItemScriptableObject()
    {
        var scriptableObjectTypes = GetScriptableObjectTypes(typeof(ItemModelConfig));
        ShowEditorWindow("Create an item model scriptable object", scriptableObjectTypes);
    }


    private static Type[] GetScriptableObjectTypes(Type type)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var scriptableObjectTypes = from x in executingAssembly.GetTypes()
                                    where (x.IsSubclassOf(type) || x == type) && !x.IsAbstract
                                    select x;
        return scriptableObjectTypes.ToArray();
    }

    private static void ShowEditorWindow(string windowTitle, Type[] scriptableObjectTypes)
    {
        var scriptableObjectEditorWindow = EditorWindow.GetWindow<ScriptableObjectEditorWindow>(true, windowTitle, true);
        scriptableObjectEditorWindow.ShowPopup();
        scriptableObjectEditorWindow.Types = scriptableObjectTypes;
    }

    #endregion Class Methods
}

#endif