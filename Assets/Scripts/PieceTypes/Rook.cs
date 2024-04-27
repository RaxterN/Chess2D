using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for rooks, child class of Piece
/// </summary>
public class Rook : Piece
{
	/// <summary>
	/// Gets the possible legal moves for a rook
	/// </summary>
	/// <returns>List of Vector2Int positions</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();

		moves.AddRange(GetSlidingMoves(1, 0));
		moves.AddRange(GetSlidingMoves(-1, 0));
		moves.AddRange(GetSlidingMoves(0, 1));
		moves.AddRange(GetSlidingMoves(0, -1));
		return moves;
	}
}
