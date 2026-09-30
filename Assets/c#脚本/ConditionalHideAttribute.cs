using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public int EnumValue;
    public bool BoolValue;

    public ConditionalHideAttribute(string conditionalSourceField, int enumValue)
    {
        ConditionalSourceField = conditionalSourceField;
        EnumValue = enumValue;
        BoolValue = true;
    }

    public ConditionalHideAttribute(string conditionalSourceField, bool boolValue = true)
    {
        ConditionalSourceField = conditionalSourceField;
        BoolValue = boolValue;
        EnumValue = -1;
    }
}