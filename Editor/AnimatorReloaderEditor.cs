using UnityEngine;
using UdonSharpEditor;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if !COMPILER_UDONSHARP && UNITY_EDITOR 
[CustomEditor(typeof(AnimatorSync))]
[CanEditMultipleObjects]
public class AnimatorReloadEditor : Editor
{
    SerializedProperty _parameterNames;

    void OnEnable()
    {
        _parameterNames = serializedObject.FindProperty("parameterNames");
    }

    public override void OnInspectorGUI()
    {
        // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

        AnimatorSync inspectorBehaviour = (AnimatorSync)target;

        EditorGUI.BeginChangeCheck();

        //A simple string field modification with Undo handling
        Animator newAnimatorVal = EditorGUILayout.ObjectField("Animator", inspectorBehaviour._animator, typeof(Animator), true) as Animator;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(inspectorBehaviour, "Modify Animator val");

            inspectorBehaviour._animator = newAnimatorVal;

            //if (newAnimatorVal != null)
            //{
            //    string[] parameters = new string[newAnimatorVal.parameters.Length];
            //    string[] values = new string[newAnimatorVal.parameters.Length];

            //    for(int i = 0; i < newAnimatorVal.parameters.Length; i++)
            //    {
            //        var param = newAnimatorVal.parameters[i];

            //        parameters[i] = param.name;
                    
            //        if (param.type == AnimatorControllerParameterType.Float)
            //        {
            //            values[i] = $"Float, {param.defaultFloat}";
            //        }
            //        else if (param.type == AnimatorControllerParameterType.Int)
            //        {
            //            values[i] = $"Int, {param.defaultInt}";
            //        }
            //        else if (param.type == AnimatorControllerParameterType.Bool)
            //        {
            //            values[i] = $"Bool, {param.defaultBool}";
            //        }
            //    }
            //    inspectorBehaviour.parameterNames = parameters;
            //    inspectorBehaviour.value = values;
            //}
        }

        //SerializedObject so = new SerializedObject(target);

        //SerializedProperty listProperty = so.FindProperty("parameterNames");

        //SerializedProperty listProperty2 = so.FindProperty("value");

        //if (listProperty != null)
        //    EditorGUILayout.PropertyField(listProperty, new GUIContent("parameterNames"), true);

        //if (listProperty2 != null)
        //    EditorGUILayout.PropertyField(listProperty2, new GUIContent("value"), true);

        //so.ApplyModifiedProperties();


    }
}
#endif