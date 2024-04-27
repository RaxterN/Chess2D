using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child class inheriting from Piece for Queens
/// </summary>
public class Queen : Piece
{
	/// <summary>
	/// Gets the possible legal moves for a queen
	/// </summary>
	/// <returns>List of Vector2Int's</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();

		//roook moves
		moves.AddRange(GetSlidingMoves(1, 0));
		moves.AddRange(GetSlidingMoves(-1, 0));
		moves.AddRange(GetSlidingMoves(0, 1));
		moves.AddRange(GetSlidingMoves(0, -1));

		//bishop moves
		moves.AddRange(GetSlidingMoves(1, 1));
		moves.AddRange(GetSlidingMoves(-1, -1));
		moves.AddRange(GetSlidingMoves(1, -1));
		moves.AddRange(GetSlidingMoves(-1, 1));
		return moves;
	}
}
