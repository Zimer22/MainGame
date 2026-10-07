using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;

public class Item : Interactable //we'll see if items need to inherit from interactable after the interactable class rework
{
    protected string itemName;
    [SerializeField] ItemMatrix _matrix;
    [SerializeField] Inventory _selfinv;
    [SerializeField] List<Sprite> _sprite;
    protected Inventory _currentinv;
    protected NetworkVariable<int> _id = new();
    protected ItemManager Itemmanager;
    /// <summary>
    /// false = Horizontal || true = Vertical
    /// </summary>
    protected bool _rotation;
    protected bool hasInventory;
    protected int _flip = 0;


    protected new void AddToManager()
    {
        if(IsServer)
        {
            _id.Value = Itemmanager.AddItem(this);
            // print("item created/added _id = " + _id.Value);
        }
        else if (_id.Value != 0)
        {
            Itemmanager.AddExistingItem(_id.Value,this);
        }
    }
    protected void AddToManagerOffline()
    {
        _id.Value = Itemmanager.AddItem(this);
    }
    protected void itemIdOnValueChanged(int prev, int next)
    {
        Itemmanager.AddExistingItem(next,this);
    }
    /// <summary>
    /// Get Block occupied or border status. If given an index instead of a data parameter this function will return the block's spriteindex.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="data">The data that is returned by the function, 0 = occupied || 1-4 borders up,down,left,right</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Data parameter only accepts from 0 to 4</exception>

    public bool GetBlock(int row, int column, byte data)
    {
        if (_rotation == false)
        {
            switch (data)
            {
                case 0:
                    return _matrix.Horizontal[row][column].Occupied[_flip];
                case 1:
                    return _matrix.Horizontal[row][column].BUp[_flip];
                case 2:
                    return _matrix.Horizontal[row][column].BDown[_flip];
                case 3:
                    return _matrix.Horizontal[row][column].BLeft[_flip];
                case 4:
                    return _matrix.Horizontal[row][column].BRight[_flip];
                default: throw new ArgumentOutOfRangeException("This only accepts from 0 to 4");
            }

        }
        else
        {
            switch (data)
            {
                case 0:
                    return _matrix.Vertical[row][column].Occupied[_flip];
                case 1:
                    return _matrix.Vertical[row][column].BUp[_flip];
                case 2:
                    return _matrix.Vertical[row][column].BDown[_flip];
                case 3:
                    return _matrix.Vertical[row][column].BLeft[_flip];
                case 4:
                    return _matrix.Vertical[row][column].BRight[_flip];
                default: throw new ArgumentOutOfRangeException("This only accepts from 0 to 4");
            }
        }
    }
    /// <summary>
    /// If not given a data parameter this function will return the block's _sprite
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public Sprite GetBlock(int index)
    {
        if (_rotation == false)
        {
            (int column, int row) = _matrix.HorizontalIndex[index];
            print(_sprite[_matrix.Horizontal[row][column].SpriteIndex[_flip]]);
            if (_sprite.Count == 0)
            {
                return null;
            }
            return _sprite[_matrix.Horizontal[row][column].SpriteIndex[_flip]];
        }
        else
        {
            (int column, int row) = _matrix.VerticalIndex[index];
            if (_sprite.Count == 0)
            {
                return null;
            }
            return _sprite[_matrix.Vertical[row][column].SpriteIndex[_flip]];
        }
    }
    public ItemMatrix GetWholeMatrix()
    {
        return _matrix;
    }
    public NetworkVariable<int> Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public ItemBlock[][] Matrix //todo: Create monobehaviour ItemEMatrix (e for editable) and functions to work with it so we allow for items with modifiable slots
    {
        get 
        {
            if (_rotation == false)
            {
                return _matrix.Horizontal;
            }
            else
            {
                return _matrix.Vertical;
            }
        }
    }

    public bool Rotation
    {
        get { return _rotation; }
        set { _rotation = value; }
    }
    public int Flip
    {
        get { return _flip; }
        set {  _flip = value; }
    }
    protected void InitializeItem()
    {
        Itemmanager = ItemManager.Instance;
        manager = ObjectManager.Instance;
        AddToManager();
        _id.OnValueChanged += itemIdOnValueChanged; //redo this whole thing so i can get rid of unnecesary events
    }
    protected void InitializeItemOffline()
    {
        Itemmanager = ItemManager.Instance;
        manager = ObjectManager.Instance;
        AddToManagerOffline();
        _id.OnValueChanged += itemIdOnValueChanged; //redo this whole thing so i can get rid of unnecesary events
    }
    private new void OnDestroy()
    {
        _id.OnValueChanged -= itemIdOnValueChanged;
        print("ItemDestroyed");
    }
}
