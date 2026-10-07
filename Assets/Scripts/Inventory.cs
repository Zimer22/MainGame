using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : NetworkBehaviour
{
    [SerializeField] protected int _columns;
    [SerializeField] protected int _rows;
    protected int[][] _matrix; //replace with custom class like itemmatrix
    protected Dictionary<int, List<(int, int)>> _savedItemPositions = new();
    protected List<int> _itemsInInv = new(); 
    protected List<(int, int)> _itemsOutInv = new();
    protected NetworkVariable<int> _id = new();
    [SerializeField] protected InventoryGrid _invGrid;
    protected ItemManager manager;
    public override void OnNetworkSpawn()
    {
        manager = ItemManager.Instance;
        CreateInventory();
        AddToManager();
        _id.OnValueChanged += itemIdOnValueChanged; 
    }
    public void CreateInventory() //for complex figures use TrimInventory AFTER this one
    {
        _matrix = new int[_rows][];
        for (int i = 0; i < _rows; i++)
        {
            _matrix[i] = new int[_columns];
            for (int k = 0; k < _columns; k++)
            {
                _matrix[i][k] = -1;
            }
        }
    }
    /// <summary>
    /// Trim inventory slot by slot or an entire row at once
    /// </summary>
    /// <param name="type"></param>
    /// -2: unavailable -3: damaged -4: upgradable
    /// <param name="trimWhole"></param>
    /// Trim whole row?
    public void TrimInventory(int row, int column,int type = -2, bool trimWhole = false)
    {
        if (trimWhole)
        {
            foreach (int i in Matrix[row])
            {
                Matrix[row][i] = type;
            }
        }
        else { Matrix[row][column] = type; }
    }
    public bool DoesItemFit(Item item, int selectedRow, int selectedColumn) //Serverside check, Then client move
    {
        for (int i = 0; i < item.Matrix.Length; i++)
        {
            for (int j = 0; j < item.Matrix[i].Length; j++)
            {
                if ((i + selectedRow + 1) > Matrix.Length || (j + selectedColumn + 1) > Matrix[i].Length) //check inventory bounds
                {
                    if (false) //IMPLEMENT. Item.ActiveRows and Item.ActiveColumns check for monobehaviour (dynamic) item sizes
                    {

                    }
                    else return false;
                }
                else if (item.GetBlock(selectedRow, selectedColumn,0) == false || Matrix[i + selectedRow][j + selectedColumn] == -1) { }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
    [Rpc(SendTo.Everyone)]
    public void InsertItemRpc(ulong itemid, int selectedRow, int selectedColumn) //Assumes all serverside checks done
    {
        Item item = manager.Items[itemid];
        _savedItemPositions.Add(item.Id.Value, new());
        for (int i = 0; i < item.Matrix.Length; i++)
        {
            for (int j = 0; j < item.Matrix[i].Length; j++)
            {
                if (item.GetBlock(i,j,0) == true)
                {
                    Matrix[i + selectedRow][j + selectedColumn] = item.Id.Value;
                    _savedItemPositions[item.Id.Value].Add((i + selectedRow, j + selectedColumn));
                }
            }
        }
        _itemsInInv.Add(item.Id.Value);
        _invGrid.AssignIds(item.Id.Value);
    }
    [Rpc(SendTo.Everyone)]
    public void DeleteItemFromInvRpc(int id) 
    {
        int row;
        int column;
        for (int i = 0; i < _savedItemPositions[id].Count; i++)
        {
            (row, column) = _savedItemPositions[id][i];
            Matrix[row][column] = -1; //There should be a system to keep in mind which slots are destroyed
            _itemsOutInv.Add(_savedItemPositions[id][i]);
        }
        _invGrid.AssignIds(_itemsOutInv);
        _itemsOutInv.Clear();
        RemoveItemFromDictionaries(id);
    }
    public void DropInvItem(int id) //needs more work
    {
        Item item = manager.GetItem(id);
        item.transform.position = gameObject.transform.forward;
        DeleteItemFromInvRpc(id);
    }
    [Rpc(SendTo.Server)]
    public void AddItemToInventoryRpc(ulong itemid, int selectedRow, int selectedColumn) 
    {
        
        if (DoesItemFit(manager.Items[itemid], selectedRow, selectedColumn)) //server doublecheck, the visual updates for item fits are handled clientside, this is a backup
        {
            InsertItemRpc(itemid, selectedRow, selectedColumn);
        }
        else { print("Serverside Itemfit check failed on position Row = " + selectedRow + " Column = " + selectedColumn); } //Add ingame error message
    }
    [Rpc(SendTo.Server)]
    public void AddItemToInventoryRpc(ulong itemid)
    {
        AutoFitItem(itemid);
    }
    public void InsertItemNonRpc(Item item, int selectedRow, int selectedColumn) //only for debug 
    {
        _savedItemPositions.Add(item.Id.Value, new());
        for (int i = 0; i < item.Matrix.Length; i++)
        {
            for (int j = 0; j < item.Matrix[i].Length; j++)
            {
                if (item.GetBlock(i, j, 0) == true)
                {
                    Matrix[i + selectedRow][j + selectedColumn] = item.Id.Value;
                    _savedItemPositions[item.Id.Value].Add((i + selectedRow, j + selectedColumn));
                }
            }
        }
        _itemsInInv.Add(item.Id.Value);
        _invGrid.AssignIds(item.Id.Value);
    }
    public void DeleteItemFromInvNonRpc(int id) //only for debug 
    {
        int row;
        int column;
        for (int i = 0; i < _savedItemPositions[id].Count; i++)
        {
            (row, column) = _savedItemPositions[id][i];
            Matrix[row][column] = -1; //There should be a system to keep in mind which slots are destroyed
            _itemsOutInv.Add(_savedItemPositions[id][i]);
        }
        _invGrid.AssignIds(_itemsOutInv);
        _itemsOutInv.Clear();
        RemoveItemFromDictionaries(id);
    }
    public int[][] Matrix
    {
        get { return _matrix; }
    }
    public List<int> ItemsInInv
    {
        get { return _itemsInInv; }
    }
    public Dictionary<int, List<(int,int)>> SavedItemPositions
    {
        get { return _savedItemPositions; }
    }
    public int Columns
    {
        get { return _columns; }
        set { _columns = value; }
    }
    public int Rows
    {
        get { return _rows; }
        set { _rows = value; }
    }
    public NetworkVariable<int> Id
    {
        get { return _id; }
        set { _id = value; }
    }
    private new void OnDestroy()
    {
        _id.OnValueChanged -= itemIdOnValueChanged;
    }
    void AddToManager()
    {
        if (IsServer)
        {
            _id.Value = manager.AddInventory(this);
            // print("item created/added _id = " + _id.Value);
        }
        else if (_id.Value != 0)
        {
            manager.AddExistingInventory(_id.Value, this);
        }
    }
    protected void itemIdOnValueChanged(int prev, int next)
    {
        manager.AddExistingInventory(next, this);
    }
    void LootTable()
    {
        //unimplemented, use tags to define loottable and add Items to spawn here OR make an inventorymanager do it for you
        //also this should not be on this class, maybe
    }
    void RemoveItemFromDictionaries(int id)
    {
        _itemsInInv.Remove(id);
        _savedItemPositions.Remove(id);
    }

    void AutoFitItem(ulong itemid, bool heavycalc = false) //heavycalc tests every possible orientation per slot, default only tests 2
    {
        Item item = manager.Items[itemid];
        bool orotation = item.Rotation;
        int oflip = item.Flip;
        for (int i = 0; i < Matrix.Length; i++)
        {
            for (int j = 0; j < Matrix[i].Length; j++)
            {
                if (heavycalc == false)
                {
                    item.Rotation = false;
                    item.Flip = 0;
                    if (DoesItemFit(item, i, j))
                    {
                        InsertItemRpc(itemid, i, j);
                        return;
                    }
                    else
                    {
                        item.Rotation = true;
                        if (DoesItemFit(item, i, j))
                        {
                            InsertItemRpc(itemid, i, j);
                            return;
                        }
                    }
                }
                else
                {
                    item.Rotation = false;
                    item.Flip = 0;
                    for (i = 0; i < 4; ++i, item.Flip++)
                    {
                        if (DoesItemFit(item, i, j))
                        {
                            InsertItemRpc(itemid, i, j);
                            return;
                        }
                    }
                    item.Rotation = true;
                    item.Flip = 0;
                    for (i = 0; i < 4; ++i, item.Flip++)
                    {
                        if (DoesItemFit(item, i, j))
                        {
                            InsertItemRpc(itemid, i, j);
                            return;
                        }
                    }
                }
            }
        }
        item.Rotation = orotation;
        item.Flip = oflip;
        print("autofit failed with heavymode = " + heavycalc);
    } 
}
