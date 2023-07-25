using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomPropertyDrawer(typeof(TechType))]
public class TechTypeDrawer : PropertyDrawer
{
    private AdvancedStringOptionsDropdown _dropdown;
    private SerializedProperty _property;
    private Rect _buttonRect;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_dropdown == null)
        {
            _dropdown = new AdvancedStringOptionsDropdown(property.enumDisplayNames);
            _dropdown.OnOptionSelected += OnDropdownOptionSelected;
        }

        position = EditorGUI.PrefixLabel(position, label);

        if (Event.current.type == EventType.Repaint)
            _buttonRect = position;

        if (GUI.Button(
                position,
                new GUIContent(property.enumDisplayNames[property.enumValueIndex]),
                EditorStyles.popup
            ))
        {
            _dropdown.Show(_buttonRect);
            _property = property;
        }
    }

    private void OnDropdownOptionSelected(int index)
    {
        _property.enumValueIndex = index;
        _property.serializedObject.ApplyModifiedProperties();
    }
}

public class AdvancedStringOptionsDropdown : AdvancedDropdown
{
    private string[] _enumNames;

    public event Action<int> OnOptionSelected;

    public AdvancedStringOptionsDropdown(string[] stringOptions) : base(new AdvancedDropdownState())
    {
        _enumNames = stringOptions;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        OnOptionSelected?.Invoke(item.id);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("");

        for (int i = 0; i < _enumNames.Length; i++)
        {
            var item = new AdvancedDropdownItem(_enumNames[i])
            {
                id = i
            };

            root.AddChild(item);
        }

        return root;
    }
}