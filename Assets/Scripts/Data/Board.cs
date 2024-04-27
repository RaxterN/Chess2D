using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the representation of the chessboard as an 8x8 array and methods for manipulating and 
/// retrieving data from it
/// </summary>
public class Board
{
	//8x8 array of pieces to represent the board
	public Piece[,] board = new Piece[8, 8];

	/// <summary>
	/// Records a piece at a given position
	/// </summary>
	/// <param name="piece">The piece in question</param>
	/// <param name="x">X position</param>
	/// <param name="y">Y position</param>
	public void SetPosition(Piece piece, int x, int y)
	{
		Debug.Log($"Setting position: ({x}, {y}) for piece {piece.GetType().Name}");
		board[x, y] = piece;
	}

	/// <summary>
	/// Clears a position on the board, marking it empty
	/// </summary>
	/// <param name="x">X position</param>
	/// <param name="y">Y position</param>
	public void ClearPosition(int x, int y)
	{
		Debug.Log($"Clearing position: ({x}, {y})");
		board[x, y] = null;
	}

	/// <summary>
	/// Returns the Piece, if any, at the given position
	/// </summary>
	/// <param name="x">X position</param>
	/// <param name="y">Y position</param>
	/// <returns>Piece</returns>
	public Piece GetPieceAtPosition(int x, int y)
	{
		return board[x, y];
	}

	/// <summary>
	/// Checks a coordinate to make sure it is within the bounds of the board/ board array
	/// </summary>
	/// <param name="x">x coordinate</param>
	/// <param name="y">y coordinate</param>
	/// <returns>bool</returns>
	public bool IsMoveWithinBounds(int x, int y)
	{
		if (x >= 0 && x <= 7 && y>= 0 && y <= 7)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
