#if UNITY_EDITOR

using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public ButtonAttribute(string label = "")
    {
        this.Label = label;
    }
}

[CustomEditor(typeof(UnityEngine.Object), true)]
[CanEditMultipleObjects]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ButtonAttribute>();
            string label = string.IsNullOrEmpty(attr.Label) ? method.Name : attr.Label;

            if (GUILayout.Button(label))
            {
                method.Invoke(target, null);
            }
        }
    }
}

#endif