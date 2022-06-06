using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Conditionally Show/Hide normal field in inspector, based on some other field value.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    #region Members

    public readonly string fieldToCheck;
    public readonly bool inverse;
    public readonly string[] compareValues;

    #endregion Members

    #region Class Methods

    /// <param name="fieldToCheck">String name of field to check value</param>
    /// <param name="inverse">Inverse check result</param>
    /// <param name="compareValues">On which values field will be shown in inspector</param>
    public ConditionalFieldAttribute(string fieldToCheck, params object[] compareValues)
    {
        this.fieldToCheck = fieldToCheck;
        this.compareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
        this.inverse = false;
    }

    public ConditionalFieldAttribute(bool inverse, string fieldToCheck, params object[] compareValues)
    {
        this.fieldToCheck = fieldToCheck;
        this.inverse = inverse;
        this.compareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
    }

    #endregion Class Methods
}