using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for knights - inherits from Piece
/// </summary>
public class Knight : Piece
{
	/// <summary>
	/// Gets the possible legal moves for a knight
	/// </summary>
	/// <returns>List of vector2int's</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();

		//this tuple holds all the permutations for a knight's moves
		(int, int)[] permutations = 
			{
				(2, 1), (1, 2), (-1, 2), (-2, 1),
				(-2, -1), (-1, -2), (1, -2), (2, -1)
			};

		for (int i = 0; i < 8; i++)
		{
			int xPos = currentPosition.x + permutations[i].Item1;
			int yPos = currentPosition.y + permutations[i].Item2;
			Vector2Int move = new Vector2Int(xPos, yPos);

			if (boardManager.IsWithinBounds(move))
			{
				if (boardManager.GetPieceAtPosition(move) == null)
				{
					moves.Add(move);
				}
				else if (boardManager.GetPieceAtPosition(move).color != color)
				{
					moves.Add(move);
				}
			}
		}
		return moves;
	}
}

//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
//I found this data type and thought it was good for this purpose
