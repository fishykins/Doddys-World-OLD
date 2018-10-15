using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace dw.planet
{
    [CustomEditor(typeof(Planet))]
    public class PlanetEditor : Editor
    {

        Planet planet;
        Editor shapeEditor;
        Editor colourEditor;
        Editor enviromentEditor;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope()) {
                base.OnInspectorGUI();
                if (check.changed) {
                    //planet.CreateDebugPlanet();
                }
            }

            if (GUILayout.Button("Generate Planet")) {
                planet.CreateDebugPlanet();
            }

            if (GUILayout.Button("Run Gravity Simulation")) {
                planet.InitializeBody(true);
            }

            DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
            DrawSettingsEditor(planet.graphicalSettings, planet.OnGraphicalSettingsUpdated, ref planet.graphicalSettingsFoldout, ref colourEditor);
            DrawSettingsEditor(planet.enviromentalSettings, planet.OnEnviromentalSettingsUpdated, ref planet.enviromentalSettingsFoldout, ref enviromentEditor);
        }

        void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
        {

            if (settings != null) {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    if (foldout) {
                        CreateCachedEditor(settings, null, ref editor);
                        editor.OnInspectorGUI();

                        if (check.changed) {
                            if (onSettingsUpdated != null) {
                                onSettingsUpdated();
                            }
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            planet = (Planet)target;
        }
    }
}
