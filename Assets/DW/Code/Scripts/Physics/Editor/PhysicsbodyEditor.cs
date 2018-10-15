using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw
{
    [CustomEditor(typeof(Physicsbody))]
    public class PhysicsbodyEditor : Editor
    {
        #region Variables
        private Physicsbody targetInput;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            targetInput = (Physicsbody)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            string debugInfo = "";
            debugInfo += "Gravity Strength: " + targetInput.GravityStrength + "\n";


            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(40));
            GUILayout.Space(20);

            Repaint();
        }
        #endregion;
    }
}