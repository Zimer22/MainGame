using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
[CreateAssetMenu(fileName = "ItemMatrix", menuName = "Scriptable Objects/ItemMatrix")]
public class ItemMatrix : ScriptableObject
{
    [SerializeField] ItemBlock[][] _horizontal;
    List<(int,int)> _horizontalIndex = new();
    [SerializeField] ItemBlock[][] _vertical;
    List<(int, int)> _verticalIndex = new();
    public ItemBlock[][] EditHorizontal //this distinction is important YOU ARE NOT SUPPOSED TO EDIT MATRIX AT RUNTIME
    {
        get {  return _horizontal; }
        set { _horizontal = value; }
    }
    public ItemBlock[][] EditVertical 
    {
        get { return _vertical; }
        set { _vertical = value; }
    }
    public ItemBlock[][] Horizontal { get => _horizontal;}
    public ItemBlock[][] Vertical { get => _vertical;}
    public List<(int, int)> HorizontalIndex
    {
        get => _horizontalIndex;
        set => _horizontalIndex = value;
    }
    public List<(int, int)> VerticalIndex
    {
        get => _verticalIndex;
        set => _verticalIndex = value;
    }
    public void SaveData()
    {
        EditorUtility.SetDirty(this);
    }
}
