using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for bishops, inherits from Piece class
/// </summary>
public class Bishop : Piece
{
	/// <summary>
	/// Gets the possible legal moves for a bishop
	/// </summary>
	/// <returns>List of Vector2Ints</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();

		moves.AddRange(GetSlidingMoves(1, 1));
		moves.AddRange(GetSlidingMoves(-1, -1));
		moves.AddRange(GetSlidingMoves(1, -1));
		moves.AddRange(GetSlidingMoves(-1, 1));
		return moves; 
	}
}
