using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw {
    [CustomEditor(typeof(InputBase))]
    public class InputEditor : Editor
    {
        #region Variables
        private InputBase targetInput;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            targetInput = (InputBase)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            string debugInfo = "";
            debugInfo += "Depth: " + targetInput.Depth + "\n";
            debugInfo += "Width: " + targetInput.Width + "\n";
            debugInfo += "Lean: " + targetInput.Lean + "\n";
            debugInfo += "Height: " + targetInput.Height + "\n";
            debugInfo += "RotateH: " + targetInput.RotateH + "\n";
            debugInfo += "RotateV: " + targetInput.RotateV + "\n";


            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
            GUILayout.Space(20);

            Repaint();
        }
        #endregion;
    }
}