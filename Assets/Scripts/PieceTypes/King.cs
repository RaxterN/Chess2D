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
		(int, int)[] permutations =
			{
				(0, 1), (1, 1), (1, 0), (1, -1),
				(0, -1), (-1, -1), (-1, 0), (-1, 1)
			};

		//Check for castling moves
		if (!hasMoved)
		{
			Debug.Log("Checking for castling moves:");
			//Kingside
			if (CanCastle(7, currentPosition.y))
			{
				moves.Add(new Vector2Int(6, currentPosition.y));
			}
			//Queenside
			if (CanCastle(0, currentPosition.y))
			{
				moves.Add(new Vector2Int(2, currentPosition.y));
			}
		}

		//Regular king moves
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

	/// <summary>
	/// Checks if the current king can castle. Note that this method does not currently rule out check in the squares the king would move through or its end position
	/// </summary>
	/// <param name="rookFile">File (y) of the rook that the king would castle with</param>
	/// <param name="rank">Rank (y) of the king</param>
	/// <returns>Bool</returns>
	private bool CanCastle(int rookFile, int rank)
	{
		Piece rook = boardManager.GetPieceAtPosition(new Vector2Int(rookFile, rank));
		if (rook != null && rook.type == PieceType.Rook && !rook.hasMoved)//piece must be in rook starting position, must be a rook, and can't have moved yet
		{
			Debug.Log($"Valid rook found at castling position: {rookFile}, {rank}");
			int direction = rookFile == 7 ? 1 : -1; //if rook is on file 7, check kingside castle
			for (int file = currentPosition.x + direction; file != rookFile; file += direction) //check positions between the king and rook for obstructing pieces
			{
				if (boardManager.GetPieceAtPosition(new Vector2Int(file, rank)) != null)
				{
					Debug.Log("Obstructing pieces found");
					return false;
				}
			}
			Debug.Log("No obstructing pieces found");
			return true;
		}
		return false;
	}
}
