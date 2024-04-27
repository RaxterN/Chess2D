using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Kings - inherits fromPiece
/// </summary>
public class King : Piece
{
	/// <summary>
	/// Gets the possible legal moves for kings
	/// </summary>
	/// <returns>List of Vector2Ints</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();

		//This tuple holds all the permutations for a kings's moves
		//Originally cam up with this for the knight, but it works for the king too
		//Shouldn't use GetSlidingMoves() for this because kings only move adjacent
		(int, int)[] permutations =
			{
				(0, 1), (1, 1), (1, 0), (1, -1),
				(0, -1), (-1, -1), (-1, 0), (-1, 1)
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
