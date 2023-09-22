using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditTextFieldWindow : EditorWindow
{
    private static bool positionSetted = false;
    private string initialText = "";
    private string resultText = "";
    private System.Action<string> onFinishAction = (resStr) => { };

    public static void Show(string text, System.Action<string> onFinish)
    {
        EditTextFieldWindow window = (EditTextFieldWindow)EditorWindow.GetWindow(typeof(EditTextFieldWindow), true);

        window.initialText = text;
        window.resultText = text;
        window.onFinishAction = onFinish;

        window.Show();

        Rect windowSize = window.position;
        windowSize.width = 300.0f;
        windowSize.height = 20.0f;

        if (!positionSetted)
        {
            positionSetted = true;
            windowSize = new Rect(
                (Screen.currentResolution.width - windowSize.width) / 2,
                (Screen.currentResolution.height - windowSize.height) / 2,
                windowSize.width,
                windowSize.height);
        }

        window.position = windowSize;
    }

    private void OnDestroy()
    {
        positionSetted = false;
        initialText = "";
        onFinishAction = (resStr) => { };
    }

    void OnGUI()
    {
        GUILayout.Space(20.0f);
        resultText = GUILayout.TextField(resultText);
        GUILayout.Space(20.0f);

        bool canApply = !string.IsNullOrEmpty(resultText) && (resultText != initialText);

        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!canApply);
        if (GUILayout.Button("Apply"))
        {
            ActionUtils.ResetAndCall<string>(ref onFinishAction, resultText, null);
            Close();
        }

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        GUILayout.EndHorizontal();
    }
}
