using System;
using Unity.Properties;
using UnityEngine;
[ExecuteInEditMode]
public class MatrixMakerHelper : MonoBehaviour
{
    [SerializeField] DebugItem item;
    [SerializeField] InventoryGrid grid;
    [CreateProperty] bool button;
    [CreateProperty] bool createinventory;
    [CreateProperty] bool createitemmatrix;
    [CreateProperty] bool printdata;
    ItemBlock block;
    InventoryBlockUI blockUI;
    [CreateProperty] (int, int) _mousepos;
    [CreateProperty] string mousetext;
    //INVENTORYUIBLOCK//
    int Value;
    [CreateProperty] int Rows;
    [CreateProperty] int Columns;
    [CreateProperty] bool Unavailable;
    [CreateProperty] bool Damaged;
    [CreateProperty] bool Upgradable;
    //INVENTORYUIBLOCK//
    //ITEMBLOCK//
    bool _rotation;
    int _flip;
    int _sprite;
    bool _occupied;
    bool _bRight;
    bool _bLeft;
    bool _bUp;
    bool _bDown;
    [CreateProperty] int ItemRows;
    [CreateProperty] int ItemColumns;
    //ITEMBLOCK//
    [CreateProperty]
    bool Rotation
    {
        get { return _rotation; }
        set { _rotation = value; item.Rotation = value; InsertItem(); }
    }
    [CreateProperty]
    int Flip
    {
        get { return _flip; }
        set { _flip = value; item.Flip = value; InsertItem(); }
    }
    [CreateProperty]
    int Sprite
    {
        get { return _sprite; }
        set { _sprite = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].SpriteIndex[_flip] = value; InsertItem(); }
    }
    [CreateProperty]
    bool Occupied
    {
        get { return _occupied; }
        set { _occupied = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].Occupied[_flip] = value; InsertItem(); }
    }
    [CreateProperty]
    bool BRight
    {
        get { return _bRight; }
        set { _bRight = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].BRight[_flip] = value; InsertItem(); }
    }
    [CreateProperty]
    bool BLeft
    {
        get { return _bLeft; }
        set { _bLeft = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].BLeft[_flip] = value; InsertItem(); }
    }
    [CreateProperty]
    bool BUp 
    {
        get { return _bUp; } 
        set { _bUp = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].BUp[_flip] = value; InsertItem(); } 
    }
    [CreateProperty]
    bool BDown
    {
        get { return _bDown; }
        set { _bDown = value; item.Matrix[_mousepos.Item1][_mousepos.Item2].BDown[_flip] = value; InsertItem(); }
    }
    void RefreshVariables()
    {
        //_mousepos = (grid.MousePos.Item2,grid.MousePos.Item3); actual mouse detection, not used
        Value = grid.SavedBlockReferences[grid.SavedBlockPositions[(grid.MousePos.Item2, grid.MousePos.Item3)]].Value;  
        if (item != null)
        {
            _rotation = item.Rotation;
            _flip = item.Flip;
            UpdateBlock();
        }
    }
    void UpdateBlock() 
    {
        block = item.Matrix[_mousepos.Item1][_mousepos.Item2];
        _occupied = block.Occupied[_flip];
        _bRight = block.BRight[_flip];
        _bLeft = block.BLeft[_flip];
        _bUp = block.BUp[_flip];
        _bDown = block.BDown[_flip];
        _sprite = block.SpriteIndex[_flip];
    }
    void GetUIBlockData() 
    {
        switch (Value)
        {
            case -2:
                Unavailable = true; Damaged = false; Upgradable = false; break;
            case -3:
                Damaged = true; Unavailable = false; Upgradable = false; break;
            case -4: 
                Upgradable = true; Unavailable = false; Damaged = false; break;
            default: Unavailable = false; Damaged = false; Upgradable = false; break;
        }
    }
    void PrintData()
    {
        for (int i = 0; i < grid.SavedBlockReferences.Count; i++ )
        {
            print(item.GetBlock(grid.SavedBlockCoordinates[i].Item1, grid.SavedBlockCoordinates[i].Item2, 0));
        }
    }
    void InsertItem()
    {
        if (grid.Inventory.ItemsInInv.Contains(item.Id.Value))
        {
            grid.Inventory.DeleteItemFromInvNonRpc(item.Id.Value);
        }
        grid.Inventory.InsertItemNonRpc(item, 0, 0);
        grid.UpdateAllBlocks();
    }
    public void CreateInv()
    {
        Inventory inv = grid.Inventory;
        inv.Rows = Rows;
        inv.Columns = Columns;
        inv.CreateInventory();
        int total = Rows * Columns;
        if (total != grid.transform.childCount)
        {
            total -= grid.transform.childCount;
            for (int i = 0; i < total; i++)
            {
                grid.AddBlock();
            }
        }
        grid.SetupLayout();
    }
    public void CreateItemMatrix()
    {
        print("creating matrix");
        if (_rotation)
        {
            print("1nd pass");
            item.GetWholeMatrix().EditVertical = new ItemBlock[ItemRows][];
            for (int j = 0; j < ItemRows; j++)
            {
                print("2rst pass");
                item.GetWholeMatrix().EditVertical[j] = new ItemBlock[ItemColumns];
                for (int k = 0; k < ItemColumns; k++)
                {

                    print("created block at row = " + j + "column = " + k);
                    item.GetWholeMatrix().EditVertical[j][k] = new ItemBlock();
                    item.GetWholeMatrix().VerticalIndex.Add((j, k));
                }
            }
        }
        else
        {
            print("1rst pass");
            item.GetWholeMatrix().EditHorizontal = new ItemBlock[ItemRows][];
            for (int j = 0; j < ItemRows; j++)
            {
                print("2nd pass");
                item.GetWholeMatrix().EditHorizontal[j] = new ItemBlock[ItemColumns];
                for (int k = 0; k < ItemColumns; k++)
                {
                    print("created block at row = " + j + "column = " + k);
                    item.GetWholeMatrix().EditHorizontal[j][k] = new ItemBlock();
                    item.GetWholeMatrix().HorizontalIndex.Add((j, k));
                }
            }
        }
        InsertItem();
    }
    public (int,int) MousePos
    {
        get {return _mousepos;}
        set 
        {
            (int, int) prevpos = _mousepos;
            _mousepos = value;
            mousetext = _mousepos.Item1 + " " + _mousepos.Item2;
            RefreshVariables();
            ChangeBlockAlpha(prevpos.Item1, prevpos.Item2, false);
            ChangeBlockAlpha(_mousepos.Item1,_mousepos.Item2,true);
        }
    } 
    public void ChangeBlockAlpha(int row, int column, bool enable)
    {
        Inventory inv = grid.Inventory;
        if (enable)
        {
            if (inv.Matrix[row][column] > -1)
            {
                foreach (var item in inv.SavedItemPositions[inv.Matrix[row][column]])
                {
                    grid.SavedBlockReferences[grid.SavedBlockPositions[item]].SetBackgroundAlpha(0.2f);
                }
            }
            else
            {
                grid.SavedBlockReferences[grid.SavedBlockPositions[(row, column)]].SetBackgroundAlpha(0.2f);
            }
        }
        else
        {
            if (inv.Matrix[row][column] > -1)
            {
                foreach (var item in inv.SavedItemPositions[inv.Matrix[row][column]])
                {
                    grid.SavedBlockReferences[grid.SavedBlockPositions[item]].SetBackgroundAlpha(0);
                }
            }
            else
            {
                grid.SavedBlockReferences[grid.SavedBlockPositions[(row,column)]].SetBackgroundAlpha(0);
            }
        }

    }
    private void Update()
    {
        if (button)
        {
            RefreshVariables();
            button = false;
        }
        if (createinventory)
        {
            print("creating");
            CreateInv();
            createinventory = false;
        }
        if (createitemmatrix)
        {
            CreateItemMatrix();
            createitemmatrix = false;
        }
        if (printdata)
        {
            PrintData();
            printdata = false;
        }
    }
}
