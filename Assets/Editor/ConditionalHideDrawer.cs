using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute), true)]
public class ConditionalHideDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property)) return;
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property)) return 0f;
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var attrs = fieldInfo
            .GetCustomAttributes(typeof(ConditionalHideAttribute), true)
            .Cast<ConditionalHideAttribute>()
            .ToArray();

        if (attrs.Length == 0) return true;

        for (int i = 0; i < attrs.Length; i++)
        {
            if (IsMatch(property, attrs[i])) return true;
        }

        return false;
    }

    private bool IsMatch(SerializedProperty property, ConditionalHideAttribute attr)
    {
        string conditionPath = property.propertyPath.Replace(property.name, attr.ConditionalSourceField);
        SerializedProperty conditionProp = property.serializedObject.FindProperty(conditionPath);
        if (conditionProp == null) return true;

        if (conditionProp.propertyType == SerializedPropertyType.Enum)
        {
            return conditionProp.enumValueIndex == attr.EnumValue;
        }

        if (conditionProp.propertyType == SerializedPropertyType.Boolean)
        {
            return conditionProp.boolValue == attr.BoolValue;
        }

        return true;
    }
}