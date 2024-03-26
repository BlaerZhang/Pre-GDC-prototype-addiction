using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace LTNGames
{
    public class FourColorDynamicGradientInspector : ShaderGUI {
    private enum Preset {
        None,
        RedGreenBlueYellow,
        CyanMagentaYellow,
        WhiteGreyBlack,
        Pastel
    }

    private static readonly Dictionary<Preset, (string, Color[])> Presets = new Dictionary<Preset, (string, Color[])> {
        { Preset.RedGreenBlueYellow, ("RedGreenBlueYellow", new Color[] {Color.red, Color.green, Color.blue, Color.yellow}) },
        { Preset.CyanMagentaYellow, ("CyanMagentaYellow", new Color[] {Color.cyan, Color.magenta, Color.yellow, Color.black}) },
        { Preset.WhiteGreyBlack , ("WhiteGreyBlack", new Color[] {Color.white, Color.grey, Color.black, Color.white}) },
        { Preset.Pastel, ("Pastel", new Color[] {HexToColor("#FF4D39"), HexToColor("#61EAFF"), HexToColor("#669DFF"), HexToColor("#DE70FF")}) }

    };

    [SerializeField, HideInInspector]
    private Preset preset;
    

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) 
    {
        Material targetMat = materialEditor.target as Material;
        MaterialProperty colorPresetProp = FindProperty("_ColorPreset", properties);
        MaterialProperty animateProp = FindProperty("_IsAnimating", properties);  // Find the new property


        preset = (Preset)colorPresetProp.floatValue;

        preset = Preset.None;
        foreach (var presetOption in Presets.Keys)
        {
            if (targetMat.IsKeywordEnabled(Presets[presetOption].Item1))
            {
                preset = presetOption;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();
        preset = (Preset) EditorGUILayout.EnumPopup("Color Preset", preset);
        if (EditorGUI.EndChangeCheck()) 
        {
            materialEditor.RegisterPropertyChangeUndo("Color Preset Change");

            // Disable all preset keywords
            foreach (var presetOption in Presets.Keys)
            {
                targetMat.DisableKeyword(Presets[presetOption].Item1);
            }

            // Enable the selected preset keyword and set the colors
            if (preset != Preset.None)
            {
                targetMat.EnableKeyword(Presets[preset].Item1);
                SetColors(targetMat, Presets[preset].Item2);
            }

            colorPresetProp.floatValue = (float)preset;
            EditorUtility.SetDirty(targetMat);
        }

        animateProp.floatValue = EditorGUILayout.Toggle("IsAnimating", animateProp.floatValue == 1) ? 1 : 0;  // Draw the checkbox

        foreach (var prop in properties) {
            // materialEditor.ShaderProperty(prop, prop.displayName);

            if(prop.name != "_ColorPreset" && prop.name != "_IsAnimating")  // Exclude the color preset property
            {
                materialEditor.ShaderProperty(prop, prop.displayName);
            }
        }
    }

    private void SetColors(Material mat, Color[] colors)
    {
        mat.SetColor("_Color1", colors[0]);
        mat.SetColor("_Color2", colors[1]);
        mat.SetColor("_Color3", colors[2]);
        mat.SetColor("_Color4", colors[3]);
    }

    private static Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
}



#endif