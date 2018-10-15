using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw
{
    [CustomEditor(typeof(Drag))]
    public class DragEditor : Editor {

        #region Variables
        private Drag targetInput;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            targetInput = (Drag)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            string debugInfo = "";
            if (targetInput.DragSections.Length > 0) {
                foreach (Drag.DragInfo info in targetInput.DragSections) {
                    debugInfo += "Drag magnitude: " + info.dragForce.magnitude + "\n";
                }
            }

            debugInfo += "Sqr Velocity: " + targetInput.SqrVelocity + "\n";

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
            GUILayout.Space(20);

            Repaint();
        }
        #endregion;
    }
}