using System;
using UnityEngine;

/// <summary>
/// Conditionally Show/Hide enum flag field in inspector, based on some other field value.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConditionalEnumFlagAttribute : PropertyAttribute
{
    #region Members

    public readonly string fieldToCheck;
    public readonly bool inverse;
    public readonly int compareValue;

    #endregion Members

    #region Class Methods

    public ConditionalEnumFlagAttribute(string fieldToCheck, int compareValue)
    {
        this.fieldToCheck = fieldToCheck;
        this.compareValue = compareValue;
        this.inverse = false;
    }

    public ConditionalEnumFlagAttribute(bool inverse, string fieldToCheck, int compareValue)
    {
        this.fieldToCheck = fieldToCheck;
        this.compareValue = compareValue;
        this.inverse = inverse;
    }

    #endregion Class Methods
}