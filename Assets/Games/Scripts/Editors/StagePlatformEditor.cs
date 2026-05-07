#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YunaSpace.BridgeRace.Editor
{
    [CustomEditor(typeof(StagePlatform))]
    public class StagePlatformEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var bp = (StagePlatform)target;

            if (bp.PlatformPoints == null || bp.PlatformPoints.Count < 2)
            {
                return;
            }

            Undo.RecordObject(bp, "Edit Platform");
            List<Vector3> pathPoints = bp.GeneratePathPoints();

            // Draw bezier handles
            for (int i = 0; i < bp.PlatformPoints.Count; i++)
            {
                Vector3 worldPos = bp.transform.TransformPoint(bp.PlatformPoints[i]);
                bool isAnchor = (i % 2 == 0);
                EditorGUI.BeginChangeCheck();
                Handles.color = isAnchor ? Color.green : Color.yellow;
                float size = HandleUtility.GetHandleSize(worldPos) * (isAnchor ? 0.1f : 0.06f);

                Vector3 newPos = Handles.FreeMoveHandle(worldPos, size, Vector3.zero, Handles.DotHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    Vector3 localPos = bp.transform.InverseTransformPoint(newPos);
                    bp.PlatformPoints[i] = SnapVector(localPos, 0.5f);
                    bp.GeneratePlatform();
                }

                int next = (i + 1) % bp.PlatformPoints.Count;
                Vector3 nextWorldPos = bp.transform.TransformPoint(bp.PlatformPoints[next]);
                Handles.color = Color.white;
                Handles.DrawLine(worldPos, nextWorldPos);

                if (isAnchor)
                {
                    Vector3 mid = (worldPos + nextWorldPos) * 0.5f;
                    Handles.color = Color.blue;
                    if (Handles.Button(mid, SceneView.lastActiveSceneView.rotation, size * 0.8f, size * 0.8f, Handles.RectangleHandleCap))
                    {
                        InsertSegment(bp, i);
                        bp.GeneratePlatform();
                    }
                }
            }

            // Draw entrance handles
            for (int i = 0; i < bp.PlatformEntrances.Count; i++)
            {
                float ent = bp.PlatformEntrances[i];
                Vector3 currentPos = bp.transform.TransformPoint(GetPointOnCurve(bp, ent));

                EditorGUI.BeginChangeCheck();
                Handles.color = Color.cyan;
                float handleSize = HandleUtility.GetHandleSize(currentPos) * 0.15f;
                Vector3 newPos = Handles.FreeMoveHandle(currentPos, handleSize, Vector3.zero, Handles.RectangleHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    bp.PlatformEntrances[i] = GetClosestT(bp, pathPoints, bp.transform.InverseTransformPoint(newPos));
                    bp.GeneratePlatform();
                }
            }
        }

        private Vector3 SnapVector(Vector3 v, float snap)
        {
            return new Vector3(Mathf.Round(v.x / snap) * snap, Mathf.Round(v.y / snap) * snap, Mathf.Round(v.z / snap) * snap);
        }

        private float GetClosestT(StagePlatform bp, List<Vector3> path, Vector3 localMouse)
        {
            float minDst = float.MaxValue;
            float bestT = 0;

            for (int i = 0; i < path.Count; i++)
            {
                int next = (i + 1) % path.Count;
                Vector3 p1 = path[i];
                Vector3 p2 = path[next];

                Vector3 line = p2 - p1;
                float len = line.magnitude;
                line.Normalize();

                Vector3 v = localMouse - p1;
                float d = Vector3.Dot(v, line);
                d = Mathf.Clamp(d, 0f, len);

                Vector3 closestPointOnSegment = p1 + line * d;
                float dst = Vector3.Distance(localMouse, closestPointOnSegment);

                if (dst < minDst)
                {
                    minDst = dst;
                    float segmentT = d / len;
                    bestT = (i + segmentT) / (float)path.Count;
                }
            }
            return bestT % 1f;
        }

        private Vector3 GetPointOnCurve(StagePlatform bp, float t)
        {
            int segments = bp.PlatformPoints.Count / 2;
            float scaledT = t * segments;
            int i = Mathf.FloorToInt(scaledT) % segments;
            float localT = scaledT - Mathf.Floor(scaledT);
            Vector3 p0 = bp.PlatformPoints[i * 2];
            Vector3 p1 = bp.PlatformPoints[i * 2 + 1];
            Vector3 p2 = bp.PlatformPoints[((i * 2) + 2) % bp.PlatformPoints.Count];
            return Mathf.Pow(1 - localT, 2) * p0 + 2 * (1 - localT) * localT * p1 + Mathf.Pow(localT, 2) * p2;
        }

        private void InsertSegment(StagePlatform bp, int index)
        {
            Undo.RecordObject(bp, "Insert Segment");
            Vector3 pA = bp.PlatformPoints[index];
            Vector3 pB = bp.PlatformPoints[(index + 1) % bp.PlatformPoints.Count];
            bp.PlatformPoints.Insert(index + 1, Vector3.Lerp(pA, pB, 0.66f));
            bp.PlatformPoints.Insert(index + 1, Vector3.Lerp(pA, pB, 0.33f));
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StagePlatform bp = (StagePlatform)target;
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Entrance"))
            {
                Undo.RecordObject(bp, "Add Entrance");
                bp.PlatformEntrances.Add(0.5f);
                bp.GeneratePlatform();
            }

            if (GUILayout.Button("Force Generate Mesh")) bp.GeneratePlatform();
        }
    }
}

#endif