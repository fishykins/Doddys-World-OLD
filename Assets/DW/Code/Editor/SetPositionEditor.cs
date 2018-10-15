using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw
{
    [CustomEditor(typeof(SetPosition))]
    public class SetPositionEditor : Editor
    {
        SetPosition vehicle;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Set Position")) {
                vehicle.SetPos(vehicle.Position);
            }

            if (GUILayout.Button("Random Position")) {
                vehicle.SetPos(new Vector3(Random.Range(-90f, 90f), Random.Range(-180f, 180f), vehicle.Position.z));
            }
        }

        private void OnEnable()
        {
            vehicle = (SetPosition)target;
        }
    }
}