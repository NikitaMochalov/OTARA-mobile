using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(Spawner))]
#endif
public class SpawnerEditor : Editor
{
#if UNITY_EDITOR
    Spawner spawner;

    private void OnEnable()
    {
        spawner = (Spawner)target;
    }

    void OnSceneGUI()
    {
        if (spawner.GUISettings.enableSceneGUI)
        {
            var verts = new Vector3[4];

            var pos = spawner.transform.position;

            verts[0] = new Vector3(pos.x - spawner.settings.areaHalfLength, pos.y, pos.z + spawner.settings.areaHalfLength);
            verts[1] = new Vector3(pos.x + spawner.settings.areaHalfLength, pos.y, pos.z + spawner.settings.areaHalfLength);
            verts[2] = new Vector3(pos.x + spawner.settings.areaHalfLength, pos.y, pos.z - spawner.settings.areaHalfLength);
            verts[3] = new Vector3(pos.x - spawner.settings.areaHalfLength, pos.y, pos.z - spawner.settings.areaHalfLength);

            Color faceColor = spawner.GUISettings.rectColor;
            Color outlineColor = Color.black;

            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            var spawnPoints = spawner.CalculatePoints();

            // Spawn the grass prefab at every determined point
            var centerPos = spawner.transform.position;
            foreach (var point in spawnPoints)
            {
                Handles.color = spawner.GUISettings.spawPointsColor;
                Handles.DrawWireCube(new Vector3(point.x, 0, point.y) + centerPos, Vector3.one * 0.5f);
            }
        }
    }
#endif
}