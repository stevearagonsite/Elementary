using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Yapp
{

    public class PhysicsExtension 
    {
        #region Properties
        SerializedProperty forceApplyType;
        SerializedProperty maxIterations;
        SerializedProperty forceMinMax;
        SerializedProperty forceAngleInDegrees;
        SerializedProperty randomizeForceAngle;
        SerializedProperty simulationRunning;
        SerializedProperty simulationStepCount;

        #endregion Properties

#pragma warning disable 0414
        PrefabPainterEditor editor;
        #pragma warning restore 0414

        PrefabPainter gizmo;

        PhysicsSimulation physicsSimulation;

        public PhysicsExtension(PrefabPainterEditor editor)
        {
            this.editor = editor;
            this.gizmo = editor.GetPainter();

            forceApplyType = editor.FindProperty(x => x.physicsSettings.forceApplyType);
            maxIterations = editor.FindProperty(x => x.physicsSettings.maxIterations);
            forceMinMax = editor.FindProperty(x => x.physicsSettings.forceMinMax);
            forceAngleInDegrees = editor.FindProperty(x => x.physicsSettings.forceAngleInDegrees);
            randomizeForceAngle = editor.FindProperty(x => x.physicsSettings.randomizeForceAngle);
            simulationRunning = editor.FindProperty(x => x.physicsSettings.simulationRunning);
            simulationStepCount = editor.FindProperty(x => x.physicsSettings.simulationStepCount);

            if (physicsSimulation == null)
            {
                physicsSimulation = ScriptableObject.CreateInstance<PhysicsSimulation>();
                
            }
            physicsSimulation.ApplySettings(gizmo.physicsSettings);
        }

        public void OnInspectorGUI()
        {
            // separator
            GUILayout.BeginVertical("box");
            //addGUISeparator();

            EditorGUILayout.LabelField("Physics Settings", GUIStyles.BoxTitleStyle);

            #region Settings

            EditorGUILayout.PropertyField(forceApplyType, new GUIContent("Force Apply Type"));

            EditorGUILayout.PropertyField(forceMinMax, new GUIContent("Force Min/Max"));
            EditorGUILayout.PropertyField(forceAngleInDegrees, new GUIContent("Force Angle (Degrees)"));
            EditorGUILayout.PropertyField(randomizeForceAngle, new GUIContent("Randomize Force Angle"));

            #endregion Settings

            EditorGUILayout.Space();

            #region Simulate Once

            EditorGUILayout.LabelField("Simulate Once", GUIStyles.GroupTitleStyle);

            EditorGUILayout.PropertyField(maxIterations, new GUIContent("Max Iterations"));
            if(maxIterations.intValue < 0)
            {
                maxIterations.intValue = 0;
            }

            if (GUILayout.Button("Simulate Once"))
            {
                RunSimulation();
            }

            #endregion Simulate Once

            EditorGUILayout.Space();

            #region Simulate Continuously

            EditorGUILayout.LabelField("Simulate Continuously", GUIStyles.GroupTitleStyle);

            EditorGUILayout.PropertyField(simulationStepCount, new GUIContent("Simulation Step"));

            GUILayout.BeginHorizontal();

            // colorize the button differently in case the physics is running, so that the user gets an indicator that the physics have to be stopped
            GUI.color = this.gizmo.physicsSettings.simulationRunning ? GUIStyles.PhysicsRunningButtonBackgroundColor : GUIStyles.DefaultBackgroundColor;
            if (GUILayout.Button("Start"))
            {
                StartSimulation();
            }
            GUI.color = GUIStyles.DefaultBackgroundColor;

            if (GUILayout.Button("Stop"))
            {
                StopSimulation();
            }

            GUILayout.EndHorizontal();

            #endregion Simulate Continuously

            EditorGUILayout.Space();

            #region Undo
            EditorGUILayout.LabelField("Undo", GUIStyles.GroupTitleStyle);

            if (GUILayout.Button("Undo Last Simulation"))
            {
                ResetAllBodies();
            }
            #endregion Undo

            GUILayout.EndVertical();
        }

        #region Physics Simulation

        private void RunSimulation()
        {
            physicsSimulation.RunSimulationOnce(getContainerChildren());
        }

        private void ResetAllBodies()
        {
            physicsSimulation.UndoSimulation();
        }

        #endregion Physics Simulation

        // TODO: create common class
        private Transform[] getContainerChildren()
        {
            if (gizmo.container == null)
                return new Transform[0];

            Transform[] children = gizmo.container.transform.Cast<Transform>().ToArray();

            return children;
        }

        private void StartSimulation()
        {
            physicsSimulation.StartSimulation(getContainerChildren());
        }

        private void StopSimulation()
        {
            physicsSimulation.StopSimulation();
        } 

    }
}
