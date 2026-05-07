using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static YunaSpace.BridgeRace.StagePlatform;

namespace YunaSpace.BridgeRace
{
    public class StagePlatform : MonoBehaviour
    {
        public List<Vector3> PlatformPoints => controlPoints;
        public List<float> PlatformEntrances => entrances;

        [SerializeField] private List<Vector3> controlPoints = new List<Vector3>()
        {
            new Vector3(-5, 0, -5), new Vector3(0, 0, -8),
            new Vector3(5, 0, -5), new Vector3(8, 0, 0),
            new Vector3(5, 0, 5), new Vector3(-8, 0, 0)
        };

        [SerializeField] private List<float> entrances = new();

        [SerializeField] private Material platformMaterial;
        [SerializeField] private Material fenceMaterial;

        [SerializeField] private bool enablePlatform = true;
        [SerializeField] private bool enableFence = true;

        private void OnValidate()
        {
            HandleGeneration();
        }

        public void BuildPlatform(List<Vector3> points, List<float> entrances)
        {
            this.controlPoints = points;
            this.entrances = entrances;

            GeneratePlatform();
        }

        public void GeneratePlatform()
        {
            List<Vector3> shapePath = GeneratePathPoints();
            int count = shapePath.Count;

            UpdateChildObject("Platform", CreatePlatformMesh(shapePath, count), platformMaterial);

            UpdateChildObject("Fence", CreateFenceMesh(shapePath, count), fenceMaterial);
        }

        public List<Vector3> GeneratePathPoints()
        {
            List<Vector3> points = new();
            for (int i = 0; i < controlPoints.Count; i += 2)
            {
                Vector3 p0 = controlPoints[i];
                Vector3 p1 = controlPoints[i + 1];
                Vector3 p2 = controlPoints[(i + 2) % controlPoints.Count];

                for (int j = 0; j < GlobalValue.PlatformCurveResolution; j++)
                {
                    float t = j / (float)GlobalValue.PlatformCurveResolution;

                    points.Add(Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2);
                }
            }
            return points;
        }

        private void HandleGeneration()
        {
            if (enablePlatform)
            {
                GeneratePlatform();
            }
            else
            {
                CleanupChild("Platform");
            }

            if (enableFence)
            {
                List<Vector3> shapePath = GeneratePathPoints();
                UpdateChildObject("Fence", CreateFenceMesh(shapePath, shapePath.Count), fenceMaterial);
            }
            else
            {
                CleanupChild("Fence");
            }
        }

        private Mesh CreatePlatformMesh(List<Vector3> path, int count)
        {
            Mesh mesh = new() { name = "Platform" };

            Vector3[] verts = new Vector3[(count * 4) + 2];
            Vector3[] norms = new Vector3[(count * 4) + 2];
            Vector2[] uvs = new Vector2[(count * 4) + 2];
            List<int> tris = new List<int>();

            Vector3 center = Vector3.zero;
            foreach (var p in path) center += p;
            center /= count;

            int topCenterIdx = count * 4;
            int bottomCenterIdx = count * 4 + 1;

            verts[topCenterIdx] = center;
            verts[bottomCenterIdx] = center + Vector3.down * GlobalValue.PlatformHeight;
            norms[topCenterIdx] = Vector3.up;
            norms[bottomCenterIdx] = Vector3.down;
            uvs[topCenterIdx] = new Vector2(center.x, center.z) * GlobalValue.PlatformUVScale;
            uvs[bottomCenterIdx] = uvs[topCenterIdx];

            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;

                verts[i] = path[i];
                verts[i + count] = path[i] + Vector3.down * GlobalValue.PlatformHeight;
                norms[i] = Vector3.up;
                norms[i + count] = Vector3.down;
                uvs[i] = new Vector2(path[i].x, path[i].z) * GlobalValue.PlatformUVScale;
                uvs[i + count] = uvs[i];

                tris.Add(topCenterIdx);
                tris.Add(next);
                tris.Add(i);

                tris.Add(bottomCenterIdx);
                tris.Add(i + count);
                tris.Add(next + count);
            }

            int sideStart = count * 2;
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                int vIdx = sideStart + (i * 2);
                verts[vIdx] = path[i];
                verts[vIdx + 1] = path[i] + Vector3.down * GlobalValue.PlatformHeight;

                Vector3 sideNorm = Vector3.Cross(Vector3.up, (path[next] - path[i]).normalized);
                norms[vIdx] = sideNorm;
                norms[vIdx + 1] = sideNorm;
                uvs[vIdx] = new Vector2(i / (float)count * 5, 1);
                uvs[vIdx + 1] = new Vector2(i / (float)count * 5, 0);

                int nextVIdx = sideStart + (next * 2);

                tris.Add(vIdx); tris.Add(nextVIdx); tris.Add(vIdx + 1);
                tris.Add(nextVIdx); tris.Add(nextVIdx + 1); tris.Add(vIdx + 1);
            }

            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = tris.ToArray();
            return mesh;
        }

        private Mesh CreateFenceMesh(List<Vector3> path, int count)
        {
            Mesh mesh = new Mesh { name = "Fence" };
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            float[] distAtPoint = new float[count];
            Vector3[] miterOffsets = new Vector3[count];
            float totalPerimeter = 0;

            for (int i = 0; i < count; i++)
            {
                distAtPoint[i] = totalPerimeter;
                Vector3 p = path[i];
                Vector3 prev = path[(i - 1 + count) % count];
                Vector3 next = path[(i + 1) % count];

                totalPerimeter += Vector3.Distance(p, next);

                Vector3 dirIn = (p - prev).normalized;
                Vector3 dirOut = (next - p).normalized;
                Vector3 normIn = Vector3.Cross(dirIn, Vector3.up);
                Vector3 normOut = Vector3.Cross(dirOut, Vector3.up);

                Vector3 miter = (normIn + normOut).normalized;

                float dot = Vector3.Dot(miter, normIn);
                float miterLength = GlobalValue.FenceThickness / Mathf.Max(0.1f, dot);
                miterOffsets[i] = miter * miterLength;
            }

            List<(float s, float e)> entranceDistRanges = new List<(float, float)>();
            foreach (var ent in entrances)
            {
                float centerDist = ent * totalPerimeter;
                float halfWidth = GlobalValue.EntranceWidth * 0.5f;
                entranceDistRanges.Add((centerDist - halfWidth, centerDist + halfWidth));
            }

            for (int i = 0; i < count; i++)
            {
                int nextIdx = (i + 1) % count;
                float dStart = distAtPoint[i];
                float dEnd = (nextIdx == 0) ? totalPerimeter : distAtPoint[nextIdx];

                List<float> splitDistances = new List<float> { dStart, dEnd };
                foreach (var range in entranceDistRanges)
                {
                    CheckAndAddSplit(splitDistances, range.s, dStart, dEnd, totalPerimeter);
                    CheckAndAddSplit(splitDistances, range.e, dStart, dEnd, totalPerimeter);
                }
                splitDistances.Sort();

                for (int j = 0; j < splitDistances.Count - 1; j++)
                {
                    float s0 = splitDistances[j];
                    float s1 = splitDistances[j + 1];
                    float mid = (s0 + s1) * 0.5f;

                    if (!IsDistanceInEntrance(mid, entranceDistRanges, totalPerimeter))
                    {
                        Vector3 p0 = GetPointAtDistance(path, distAtPoint, s0);
                        Vector3 p1 = GetPointAtDistance(path, distAtPoint, s1);

                        float t0 = (s0 - dStart) / (dEnd - dStart);
                        float t1 = (s1 - dStart) / (dEnd - dStart);
                        Vector3 off0 = Vector3.Lerp(miterOffsets[i], miterOffsets[nextIdx], t0);
                        Vector3 off1 = Vector3.Lerp(miterOffsets[i], miterOffsets[nextIdx], t1);

                        AddFenceSegment(p0, p1, off0, off1, s0, s1, verts, norms, uvs, tris);
                    }
                }
            }

            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            return mesh;
        }

        private void AddFenceSegment(Vector3 p0, Vector3 p1, Vector3 off0, Vector3 off1, float d0, float d1, List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris)
        {
            Vector3 out0 = p0 + off0;
            Vector3 out1 = p1 + off1;
            Vector3 out0Top = out0 + Vector3.up * GlobalValue.FenceHeight;
            Vector3 out1Top = out1 + Vector3.up * GlobalValue.FenceHeight;

            Vector3 in0 = p0;
            Vector3 in1 = p1;
            Vector3 in0Top = in0 + Vector3.up * GlobalValue.FenceHeight;
            Vector3 in1Top = in1 + Vector3.up * GlobalValue.FenceHeight;

            Vector3 segmentNorm = Vector3.Cross((p1 - p0).normalized, Vector3.up);

            AddFace(out1, out1Top, out0, out0Top, segmentNorm, d1, d0, verts, norms, uvs, tris);
            AddFace(in0, in0Top, in1, in1Top, -segmentNorm, d0, d1, verts, norms, uvs, tris);
            AddFace(in1Top, in0Top, out1Top, out0Top, Vector3.up, d1, d0, verts, norms, uvs, tris);
        }

        private void AddFace(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal, float u0, float u1, List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris)
        {
            int baseIdx = verts.Count;
            verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);

            for (int i = 0; i < 4; i++) norms.Add(normal);

            uvs.Add(new Vector2(u0 * GlobalValue.PlatformUVScale, 0));
            uvs.Add(new Vector2(u0 * GlobalValue.PlatformUVScale, 1));
            uvs.Add(new Vector2(u1 * GlobalValue.PlatformUVScale, 0));
            uvs.Add(new Vector2(u1 * GlobalValue.PlatformUVScale, 1));

            tris.AddRange(new int[] { baseIdx, baseIdx + 1, baseIdx + 2, baseIdx + 1, baseIdx + 3, baseIdx + 2 });
        }

        private void CheckAndAddSplit(List<float> splits, float val, float start, float end, float total)
        {
            float normalizedVal = ((val % total) + total) % total;
            if (normalizedVal > start && normalizedVal < end) splits.Add(normalizedVal);
        }

        private bool IsDistanceInEntrance(float d, List<(float s, float e)> ranges, float total)
        {
            foreach (var r in ranges)
            {
                float sd = ((r.s % total) + total) % total;
                float ed = ((r.e % total) + total) % total;

                if (sd < ed) { if (d >= sd && d <= ed) return true; }
                else { if (d >= sd || d <= ed) return true; }
            }
            return false;
        }

        private Vector3 GetPointAtDistance(List<Vector3> path, float[] distAtPoint, float d)
        {
            int count = path.Count;
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                float d0 = distAtPoint[i];
                float d1 = (next == 0) ? distAtPoint[count - 1] + Vector3.Distance(path[count - 1], path[0]) : distAtPoint[next];

                if (d >= d0 && d <= d1)
                {
                    float t = (d - d0) / (d1 - d0);
                    return Vector3.Lerp(path[i], path[next], t);
                }
            }
            return path[0];
        }

        private void UpdateChildObject(string name, Mesh mesh, Material material)
        {
            Transform child = transform.Find(name);

            if (child == null)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(this.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                child = go.transform;

                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
            }

            child.GetComponent<MeshFilter>().sharedMesh = mesh;
            child.GetComponent<MeshRenderer>().material = material;
        }

        private void CleanupChild(string name)
        {
            Transform child = transform.Find(name);
            if (child != null)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (child != null) DestroyImmediate(child.gameObject);
                };
            }
        }
    }
}