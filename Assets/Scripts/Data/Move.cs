using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This just holds the data needed to record a move
/// </summary>
public struct Move
{
    public Vector2Int startPosition;
    public Vector2Int endPosition;
    Piece piece;

	public Move(Vector2Int newPosition, Vector2Int currentPosition, Piece piece)
	{
		startPosition = currentPosition;
		endPosition = newPosition;
		this.piece = piece;
	}
}
