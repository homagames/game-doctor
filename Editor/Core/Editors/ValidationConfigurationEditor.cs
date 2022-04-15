using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    [CustomEditor(typeof(ValidationProfileConfiguration))]
    public class ValidationConfigurationEditor : Editor
    {
        private SerializedProperty checksListProperty;
        private TypeCache.TypeCollection checksTypes;
        private GenericMenu addCheckMenu;

        private void OnEnable()
        {
            checksTypes = TypeCache.GetTypesDerivedFrom<Check>();
            checksListProperty = serializedObject.FindProperty("Checks");
            addCheckMenu = new GenericMenu();
            for (int i = 0; i < checksTypes.Count; i++)
            {
                addCheckMenu.AddItem(new GUIContent(checksTypes[i].FullName), false, OnAddCheck,
                    checksTypes[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            ValidationProfileConfiguration config = target as ValidationProfileConfiguration;
            for (int i = 0; i < checksListProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (checksListProperty.GetArrayElementAtIndex(i) != null)
                    EditorGUILayout.LabelField(checksListProperty.GetArrayElementAtIndex(i).managedReferenceFullTypename);

                if (GUILayout.Button("-"))
                {
                    checksListProperty.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
            {
                addCheckMenu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddCheck(object type)
        {
            var index = checksListProperty.arraySize;
            checksListProperty.InsertArrayElementAtIndex(index);
            checksListProperty.GetArrayElementAtIndex(index).managedReferenceValue =
                Activator.CreateInstance((type as Type)!);
            Debug.Log($"Adding new check :{type}");
        }
    }
}