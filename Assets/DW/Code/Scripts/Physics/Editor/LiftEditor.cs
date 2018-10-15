using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw
{
    [CustomEditor(typeof(Lift))]
    public class LiftEditor : Editor
    {
        #region Variables
        private Lift targetInput;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            targetInput = (Lift)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            string debugInfo = "";
            debugInfo += "Air Density: " + targetInput.AirDensity + "\n";
            debugInfo += "Angle of attack: " + targetInput.AngleOfAttack + "\n";
            debugInfo += "Lift Coefficient: " + targetInput.LiftCoefficient + "\n";
            debugInfo += "Lift Strength: " + targetInput.LiftForce + "\n";


            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
            GUILayout.Space(20);

            Repaint();
        }
        #endregion

    }
}