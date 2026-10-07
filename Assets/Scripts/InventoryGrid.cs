using NUnit;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;
[ExecuteInEditMode]

public class InventoryGrid : MonoBehaviour
{
    bool setup;
    /// <summary>
    ///value1 = inventoryid the mouse is in, value2 row, value3 column
    /// </summary>
    (int, int,int) _mousepos;
    [SerializeField]GridLayoutGroup gridLayout;
    [SerializeField]Inventory _inventory;
    [SerializeField]Canvas canvas;
    [SerializeField] GameObject block;
    PlayerMenuUI _playerMenuUI;
    Dictionary<(int, int), int> _savedBlockIndexes = new();
    List<(int,int)> _savedBlockCoordinates = new();
    [SerializeField]List<InventoryBlockUI> _savedBlockReferences = new();
    public void PlayerStarted() 
    {
        CameraManager.Instance.OnCameraChange += SetCanvasCam;
        ItemManager.Instance.AddGrid(this);
        SetupLayout();
    }
    public void SetupLayout()
    {
        gridLayout.constraintCount = _inventory.Rows;   
        InitializeBlocks();
    }
    private void OnDestroy()
    {
        CameraManager.Instance.OnCameraChange -= SetCanvasCam;
    }
    void InitializeBlocks() 
    {
        int row = 0;
        int column = 0;
        int index = 0;
        foreach (InventoryBlockUI inventoryBlock in _savedBlockReferences)
        {
            if (column + 1 > _inventory.Columns)
            {
                column = 0;
                row++;
            }
            inventoryBlock.Value = _inventory.Matrix[row][column];
            inventoryBlock.EventTrigger.triggers[0].callback.AddListener(PointerHover);
            inventoryBlock.EventTrigger.triggers[1].callback.AddListener(PointerExit);
            _savedBlockIndexes.Add((row,column), index); //this relies on blocks NEVER being deleted or moved
            _savedBlockCoordinates.Add((row, column));
            column++;
            index++;
            print("initialized");
        }
    }
    public void UpdateAllBlocks()//probs just debug, forcefully update every block so their image resets. not optimal
    {
        for (int i = 0; i < SavedBlockReferences.Count; i++)
        {
            _savedBlockReferences[i].Index = _savedBlockReferences[i].Index;
            _savedBlockReferences[i].Value = _savedBlockReferences[i].Value;
            _savedBlockReferences[i].Rotation = _savedBlockReferences[i].Rotation;
        }
    } 
    public void AssignIds(int itemid)
    {
        for(int i = 0; i < Inventory.SavedItemPositions[itemid].Count; i++)
        {
            _savedBlockReferences[_savedBlockIndexes[Inventory.SavedItemPositions[itemid][i]]].Index = i;
            _savedBlockReferences[_savedBlockIndexes[Inventory.SavedItemPositions[itemid][i]]].Value = itemid;
            _savedBlockReferences[_savedBlockIndexes[Inventory.SavedItemPositions[itemid][i]]].Rotation = ItemManager.Instance.GetItem(itemid).Flip;
        }
    }
    public void AssignIds(List<(int,int)> coords)
    {
        for (int i = 0; i < coords.Count; i++)
        {
            _savedBlockReferences[_savedBlockIndexes[coords[i]]].Index = 0;
            _savedBlockReferences[_savedBlockIndexes[coords[i]]].Value = Inventory.Matrix[coords[i].Item1][coords[i].Item2];
            _savedBlockReferences[_savedBlockIndexes[coords[i]]].Rotation = 0;
        }
    }
    void PointerHover(BaseEventData eventData) //ashamed to admit copilot helped me out here, IT ALSO TRIED GASLIGHTING ME INTO LIKING IT WHY THE FUCK CAN IT AUTOCOMPLETE COMMENTS
    {
        var pointer = eventData as PointerEventData;
        if (pointer == null) return;

        // prefer the actual hovered GameObject from the event
        var hovered = pointer.pointerEnter;
        if (hovered == null) return;

        (int, int) tuple = SavedBlockCoordinates[hovered.transform.GetSiblingIndex()];
        MousePos = (Inventory.Id.Value, tuple.Item1, tuple.Item2);
        print("entered = " + MousePos);
        PlayerMenuUi.MouseEntered(this); //reminder handle assigment of playermenuui to non serialized grids
    } 
    void PointerExit(BaseEventData eventData)
    {
        var pointer = eventData as PointerEventData;
        if (pointer == null) return;

        // prefer the actual hovered GameObject from the event
        var hovered = pointer.pointerEnter;
        if (hovered == null) return;

        (int, int) tuple = SavedBlockCoordinates[hovered.transform.GetSiblingIndex()];
        MousePos = (Inventory.Id.Value, tuple.Item1, tuple.Item2);
        PlayerMenuUi.MouseLeft(this);
    }
    void SetCanvasCam(Camera cam)
    {
        canvas.worldCamera = cam;
    }
    public void AddBlock()
    {
        SavedBlockReferences.Add(Instantiate(block,transform).GetComponent<InventoryBlockUI>());
    }
    public (int,int,int) MousePos
    {
        get { return _mousepos; }
        set
        { 
            _mousepos = value;

        }
    }
    public Inventory Inventory
    {
        get {return  _inventory; }
        set { _inventory = value; }
    }
    public Dictionary<(int, int), int> SavedBlockPositions
    {
        get { return _savedBlockIndexes; }
        set { _savedBlockIndexes = value; }
    }
    public List<(int,int)> SavedBlockCoordinates
    {
        get { return _savedBlockCoordinates; }
        set { value = _savedBlockCoordinates; }
    }
    public List<InventoryBlockUI> SavedBlockReferences
    {
        get { return _savedBlockReferences; }
        set { _savedBlockReferences = value; }
    }
    public PlayerMenuUI PlayerMenuUi
    {
        get { return _playerMenuUI; }
        set { _playerMenuUI = value; }
    }
    // another solution
    //void InitializeBlocks(Inventory _inventory)
    //{
    //    int amount = _inventory._matrix.Count * _inventory._matrix[0].Count;
    //    int column = 0;
    //    int row = 0;
    //    InventoryBlockUI inventoryBlock;
    //    for (int i = 0; i < amount; i++)
    //    {
    //        for(int j = 0; j * _inventory._matrix.Count < i ; j++)
    //        {
    //            column = i - j * _inventory._matrix.Count;
    //            row = j;
    //        }
    //        inventoryBlock = Instantiate(block,rTransform).GetComponentAtIndex<InventoryBlockUI>(3);
    //        inventoryBlock.Column = column;
    //        inventoryBlock.Row = row;
    //        inventoryBlock.Id = _inventory._matrix[column][row];
    //    }
    //}
}
