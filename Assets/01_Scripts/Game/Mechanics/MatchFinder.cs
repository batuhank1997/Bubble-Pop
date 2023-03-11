using System.Collections.Generic;
using _01_Scripts.Game.Core;

namespace _01_Scripts.Game.Mechanics
{
	public class MatchFinder
	{
		private readonly bool[,] _visitedCells = new bool[Board.Cols, Board.Rows];
		
		/// <summary>
		/// Returns the list of matching cells according to the given match type.
		/// Returns only the given cell if there are no neighbour items with the given match type.
		/// </summary>
		public List<Cell> FindMatches(Cell cell, int value)
		{
			var resultCells = new List<Cell>();
			ClearVisitedCells();
			FindMatches(cell, value, resultCells);

			return resultCells;
		}

		public void FindMatches(Cell cell, int value, List<Cell> resultCells)
		{
			if (cell == null) return;
			
			var x = cell.X;
			var y = cell.Y;
			
			if (_visitedCells[x, y]) return;

			if ((cell.HasItem && cell.Item.GetValue() == value))
			{
				_visitedCells[x, y] = true;
				resultCells.Add(cell);
			
				var neighbours = cell.Neighbours;
				
				if (neighbours.Count == 0) return;
	
				for (var i = 0; i < neighbours.Count; i++)
				{	
					FindMatches(neighbours[i], value, resultCells);
				}
			}
		}

		public void ClearVisitedCells()
		{
			for (var i = 0; i < Board.Rows; i++)
			{
				for (var j = 0; j < Board.Cols; j++)
				{
					_visitedCells[j, i] = false;
				}
			}
		}
	}
}

