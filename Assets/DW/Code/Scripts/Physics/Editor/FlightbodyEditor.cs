using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw
{
    [CustomEditor(typeof(Flightbody))]
    public class FlightbodyEditor : Editor
    {
        #region Variables
        private Flightbody targetInput;
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            targetInput = (Flightbody)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            string debugInfo = "";
            debugInfo += "Gravity Strength: " + targetInput.GravityStrength + "\n";
            debugInfo += "Lift power: " + targetInput.LiftPower + "\n";
            debugInfo += "Forward Speed: " + targetInput.ForwardSpeed + "\n";
            debugInfo += "Vertical Speed: " + targetInput.VerticalSpeed + "\n";
            debugInfo += "Horizontal Speed: " + targetInput.HorizontalSpeed + "\n";
            debugInfo += "Angle of Attack: " + targetInput.AngleOfAttack + "\n";
            debugInfo += "Rigidbody Assist: " + targetInput.RbAssistPower + "\n";
            debugInfo += "Airspeed: " + targetInput.AirSpeed + "\n";
            debugInfo += "Drag: " + targetInput.DragPower + "\n";

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(150));
            GUILayout.Space(20);

            Repaint();
        }
        #endregion;
    }
}