using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Answer))]
public class AnswerEditor : Editor
{
    SerializedProperty answerTextProp;
    SerializedProperty answerObjectProp;
    SerializedProperty answerTypeProp;
    SerializedProperty numNextVideoProp;
    SerializedProperty isCorrectProp;

    private void OnEnable()
    {
       // answerTextProp = serializedObject.FindProperty("answerText");
        answerObjectProp = serializedObject.FindProperty("answerObject");
        answerTypeProp = serializedObject.FindProperty("answerType");
        numNextVideoProp = serializedObject.FindProperty("numNextVideo");
        isCorrectProp = serializedObject.FindProperty("isCorrect");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.PropertyField(answerTextProp);
        EditorGUILayout.PropertyField(answerObjectProp);
        EditorGUILayout.PropertyField(answerTypeProp);
        EditorGUILayout.PropertyField(isCorrectProp);

        // ≈сли выбран тип ответа SwitchVideo, показываем поле numNextVideo
        if (answerTypeProp.enumValueIndex == (int)AnswerType.SwitchVideo)
        {
            EditorGUILayout.PropertyField(numNextVideoProp);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
