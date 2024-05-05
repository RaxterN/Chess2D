using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pawns - inherits from Piece
/// </summary>
public class Pawn : Piece
{
	/// <summary>
	/// Gets the possible legal moves for pawns
	/// </summary>
	/// <returns>List of Vector2Int positions</returns>
	public override List<Vector2Int> GetMoves()
	{
		List<Vector2Int> moves = new List<Vector2Int>();
		int direction = color == PieceColor.White ? 1 : -1;

		//if piece is white y increments will be positive, negative if black
		//because pawns always move 'forward' from their side
		int sideMult = 1;
		if (color == PieceColor.Black)
		{
			sideMult = -1;
		}

		/* Pawns can move one ahead (if not blocked by an enemy or friendly piece) or 
		   two ahead on their very first move. Since they take forward and diagonal, check
		   those positions for enemy pieces as well. takePosition1 should be the diagonal right
		   square and takePosition2 should be to the left, relative to the side*/
		Vector2Int oneAhead = new Vector2Int(currentPosition.x, currentPosition.y + (1 * sideMult));
		Vector2Int twoAhead = new Vector2Int(currentPosition.x, currentPosition.y + (2 * sideMult));
		Vector2Int takePosition1 = new Vector2Int(currentPosition.x + 1, currentPosition.y + (1 * sideMult));
		Vector2Int takePosition2 = new Vector2Int(currentPosition.x - 1, currentPosition.y + (1 * sideMult));

		//check the possible forward move positions
		if (boardManager.IsWithinBounds(twoAhead) == true)
		{
			if (boardManager.GetPieceAtPosition(twoAhead) == null && boardManager.GetPieceAtPosition(oneAhead) == null)
			{
				if ((currentPosition.y == 1 && color == PieceColor.White) || (currentPosition.y == 6 && color == PieceColor.Black))
				{
					moves.Add(twoAhead);
				}
			}
		}
		if (boardManager.IsWithinBounds(oneAhead) == true)
		{
			if (boardManager.GetPieceAtPosition(oneAhead) == null)
			{
				moves.Add(oneAhead);
			}
		}
		//check the posible take positions

		if (boardManager.IsWithinBounds(takePosition1) == true)
		{
			if (boardManager.GetPieceAtPosition(takePosition1) != null && boardManager.GetPieceAtPosition(takePosition1).color != color)
			{
				moves.Add(takePosition1);
			}
		}
		if (boardManager.IsWithinBounds(takePosition2) == true)
		{
			if (boardManager.GetPieceAtPosition(takePosition2) != null && boardManager.GetPieceAtPosition(takePosition2).color != color)
			{
				moves.Add(takePosition2);
			}
		}

		//check for en passant opportunities 
		if (boardManager.GetTurnCount() > 0)//must have a move in the Moves stack to check
		{
			Move lastMove = boardManager.moves.Peek();
			if (boardManager.IsDoublePawnMove(lastMove)) //if the last move was double pawn move
			{
				if (lastMove.endPosition.x == currentPosition.x + 1 || lastMove.endPosition.x == currentPosition.x - 1)//if the moved pawn is next to this pawn
				{
					if (lastMove.endPosition.y == currentPosition.y)
					{
						Vector2Int enPassantCapture = new Vector2Int(lastMove.endPosition.x, lastMove.endPosition.y + direction);
						if (boardManager.IsWithinBounds(enPassantCapture))
						{
							moves.Add(enPassantCapture);
						}
					}
				}
			}
		}
		return moves;
	}
}
