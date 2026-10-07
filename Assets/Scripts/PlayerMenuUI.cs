using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
public class PlayerMenuUI : MonoBehaviour
{
    bool isHoldingItem;
    Item itemheld;
    CameraManager manager;
    Canvas canvas;
    [SerializeField] Player player;
    [SerializeField] InventoryGrid[] _grids;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    //MAX INV SIZE ON TOP ROWS(GRID 0-1-2) IS 9 COLUMNS AND 7 ROWS, BOTTOM ROWS IS 9 COLUMNS AND 5 ROWS
    void Start()
    {
        manager = CameraManager.Instance;
        manager.OnCameraChange += CameraChanged;
        canvas = GetComponent<Canvas>();
        _grids[0].PlayerMenuUi = this;
    }
    void CameraChanged(Camera cam)
    {
        canvas.worldCamera = cam;
    }
    void OnDestroy()
    {
        manager.OnCameraChange -= CameraChanged;
    }
    public void MouseEntered(InventoryGrid grid)
    {
        if (isHoldingItem)
        {

        }
        else //add additional checks if needed
        {
            ChangeBlockAlphaRpc(grid.MousePos.Item1,grid.MousePos.Item2, grid.MousePos.Item3, true);
        }
    }
    public void MouseLeft(InventoryGrid grid)
    {
        if (isHoldingItem)
        {

        }
        else //add additional checks if needed
        {
            ChangeBlockAlphaRpc(grid.MousePos.Item1, grid.MousePos.Item2, grid.MousePos.Item3, false);
        }
    }
    [Rpc(SendTo.Everyone)]
    public void ChangeBlockAlphaRpc(int invid, int column, int row, bool enable)
    {
        ItemManager itemManager = ItemManager.Instance;
        Inventory inv = itemManager.GetInventory(invid);
        InventoryGrid grid = itemManager.GetGrid(invid);
        if (enable)
        {
            if (inv.Matrix[column][row] > -1)
            {
                foreach (var item in inv.SavedItemPositions[inv.Matrix[column][row]])
                {
                    grid.SavedBlockReferences[grid.SavedBlockPositions[item]].SetBackgroundAlpha(0.2f);
                }
            }
            else
            {
                grid.SavedBlockReferences[grid.SavedBlockPositions[(column, row)]].SetBackgroundAlpha(0.2f);
            }
        }
        else
        {
        if (inv.Matrix[column][row] > -1)
        {
            foreach (var item in inv.SavedItemPositions[inv.Matrix[column][row]])
            {
                grid.SavedBlockReferences[grid.SavedBlockPositions[item]].SetBackgroundAlpha(0);
            }
        }
        else
        {
            grid.SavedBlockReferences[grid.SavedBlockPositions[(column,row)]].SetBackgroundAlpha(0);
        }
        }

    }
    public InventoryGrid[] Grids
    {
        get { return _grids;  }
        set 
        {
            _grids = value;
        }
    }
}
