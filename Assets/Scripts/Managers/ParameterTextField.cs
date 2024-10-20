using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System.Reflection;
using System;
using TMPro;
public class ParameterTextField : MonoBehaviour
{
    public TMP_InputField inputField;
    public string label = "gridSizeX";
    // Start is called before the first frame update
    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParamValue() {
        inputField.text = GetFieldValue(ParameterHandler.Instance.parameterInfo, label);
    }

    public void SetParameter() {
        SetFieldValue(ParameterHandler.Instance.parameterInfo, label, inputField.text);
    }

    static void SetFieldValue(object obj, string fieldName, object value) {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the field info for the specified field name
        FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        Type fieldType = field.FieldType;

        // Check if the field exists
        if (field != null)
        {
            // Set the field value
            object castedValue = Convert.ChangeType(value, fieldType);
            field.SetValue(obj, castedValue);
        }
        else
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{type.Name}'.");
        }
    }

    static string GetFieldValue(object obj, string fieldName) {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the field info for the specified field name
        FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        object value = field.GetValue(obj);
        // Check if the field exists
        if (field != null)
        {
            // Set the field value
            object castedValue = Convert.ChangeType(value, TypeCode.String);
            return (string)castedValue;
        }
        else
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{type.Name}'.");
        }
    }

}
