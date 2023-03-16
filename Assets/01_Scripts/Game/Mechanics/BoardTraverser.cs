using System.Collections.Generic;
using _01_Scripts.Game.Core;

public class BoardTraverser
{
    private HashSet<Cell> allVisitedCells = new HashSet<Cell>();
    private HashSet<Cell> connectedVisitedCells = new HashSet<Cell>();

    public void TraverseBoardAndKill(Board board, Cell startingCell)
    {
        ResetDFS(board);

        var _startingCell = startingCell;

        DFSAll(_startingCell);
        DFSConnecteds(_startingCell);
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
                board.Cells[j, i].isVisited = false;
            }
        }
    }

    private void DFSAll(Cell startingCell)
    {
        allVisitedCells.Add(startingCell);
        startingCell.isVisited = true;

        foreach (Cell neighbor in startingCell.neighbours)
        {
            if (!allVisitedCells.Contains(neighbor))
            {
                DFSAll(neighbor);
            }
        }
    }
    
    private void DFSConnecteds(Cell startingCell)
    {
        connectedVisitedCells.Add(startingCell);
        startingCell.isVisited = true;

        foreach (Cell neighbor in startingCell.neighbours)
        {
            if (!connectedVisitedCells.Contains(neighbor) && neighbor.hasItem)
            {
                DFSConnecteds(neighbor);
            }
        }
    }

    private void KillItemsInRemaningPart(HashSet<Cell> remainingCells)
    {
        foreach (Cell cell in remainingCells)
        {
            if (cell.hasItem)
            {
                cell.KillItem();
            }
        }
    }
}
