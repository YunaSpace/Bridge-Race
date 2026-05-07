using System;
using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public enum ColorType
    {
        None = 0,

        Red = 1,
        Blue = 2,
        Yellow = 3,
        Green = 4,
        Orange = 5,
        Purple = 6,
        Pink = 7,
        Cyan = 8,
        Brown = 9,
    }

    [Serializable]
    public class ColorData
    {
        public Color Color;
        public Material BrickMaterial;
        public Material StairMaterial;

        public ColorData(Color color)
        {
            Color = color;
        }
    }

    [CreateAssetMenu(fileName = "Color Palette SO", menuName = "Bridge Race/Color Palette SO")]
    public class ColorPaletteSO : ScriptableObject
    {
        public Material CharacterMaterial;

        public List<ColorData> Colors = new()
        {
            new(new Color(0.7f, 0.7f, 0.7f)), // None
            new(new Color(1f, 0.2f, 0.2f)),   // Red
            new(new Color(0f, 0.3f, 1f)),     // Blue
            new(new Color(0.9f, 0.9f, 0f)),   // Yellow
            new(new Color(0f, 1f, 0f)),       // Green
            new(new Color(1f, 0.5f, 0f)),     // Orange
            new(new Color(0.7f, 0f, 1f)),     // Purple
            new(new Color(1f, 0.4f, 0.7f)),   // Pink
            new(new Color(0f, 0.8f, 0.8f)),   // Cyan
            new(new Color(0.6f, 0.3f, 0.1f)), // Brown
        };

        public void InitializeMaterial()
        {
            CharacterMaterial = new Material(Shader.Find("Shader Graphs/Character Surface"));
            CharacterMaterial.enableInstancing = true;

            var brickShader = Shader.Find("Shader Graphs/Ground Brick");
            var stairShader = Shader.Find("Shader Graphs/Stair Brick");

            foreach (var entry in Colors)
            {
                var brickMaterial = new Material(brickShader);
                brickMaterial.enableInstancing = true;
                brickMaterial.SetColor("_BaseColor", entry.Color);
                entry.BrickMaterial = brickMaterial;

                var stairMaterial = new Material(stairShader);
                stairMaterial.enableInstancing = true;
                stairMaterial.SetColor("_BaseColor", entry.Color);
                entry.StairMaterial = stairMaterial;
            }
        }

        public ColorData GetColorData(ColorType type)
        {
            int index = (int)type;

            if (index < 0 || index >= Colors.Count)
            {
                Debug.LogWarning($"Color index {index} out of range.");
                return Colors[0];
            }

            return Colors[index];
        }
    }
}