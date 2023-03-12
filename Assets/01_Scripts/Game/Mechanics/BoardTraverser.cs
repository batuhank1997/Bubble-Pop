using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01_Scripts.Game.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoardTraverser
{
    public Cell StartingCell;

    HashSet<Cell> allVisitedCells = new HashSet<Cell>();
    HashSet<Cell> connectedVisitedCells = new HashSet<Cell>();

    public void TraverseBoardAndKill(Board board)
    {
        ResetDFS(board);

        var startingCell = board.Cells[0, 0];

        Debug.Log(startingCell.name);
        DFSAll(startingCell);
        DFSConnecteds(startingCell);
        var allCells = allVisitedCells;
        var connectedCells = connectedVisitedCells;
        
        var setResult = new HashSet<Cell>();
        setResult = new HashSet<Cell>(allCells);
        setResult.ExceptWith(connectedCells);

        KillItemsInRemaningPart(setResult);
    }

    private void ResetDFS(Board board)
    {
        allVisitedCells.Clear();
        connectedVisitedCells.Clear();
        
        for (int i = 0; i < Board.Rows; i++)
        {
            for (int j = 0; j < Board.Cols; j++)
            {
                board.Cells[j, i].IsVisited = false;
            }
        }
    }

    private void DFSAll(Cell startingCell)
    {
        allVisitedCells.Add(startingCell);
        startingCell.IsVisited = true;

        foreach (Cell neighbor in startingCell.Neighbours)
        {
            if (!allVisitedCells.Contains(neighbor))
            {
                DFSAll(neighbor);
            }
        }
    }
    
    private void DFSConnecteds(Cell startingCell)
    {
        // Debug.Log("Visiting cell " + startingCell.name);
        connectedVisitedCells.Add(startingCell);
        startingCell.IsVisited = true;

        foreach (Cell neighbor in startingCell.Neighbours)
        {
            if (!connectedVisitedCells.Contains(neighbor) && neighbor.HasItem)
            {
                DFSConnecteds(neighbor);
            }
        }
    }

    private void KillItemsInRemaningPart(HashSet<Cell> remainingCells)
    {
        foreach (Cell cell in remainingCells)
        {
            if (cell.HasItem)
            {
                cell.KillItem();
            }
        }
    }
}
