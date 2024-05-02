using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class that contains data related to pieces
/// </summary>
public abstract class Piece : MonoBehaviour
{
	public BoardManager boardManager;
	public PieceColor color;
	public PieceType type;
	public Vector2Int currentPosition;


	// Start is called before the first frame update
	void Awake()
	{
		//get boardManager reference on piece instantiation
		//this works because there will only ever be one BoardManager in the scene
		boardManager = FindObjectOfType<BoardManager>();
	}

	/// <summary>
	/// Sets the initial position of a piece, calling BoardManager to place the piece without
	/// recording a move or starting the next turn. Use this for initializing pieces
	/// </summary>
	/// <param name="position"></param>
	public void SetInitialPosition(Vector2Int position)
	{
		currentPosition = position;
		boardManager.PlacePiece(position, this);
	}

	/// <summary>
	/// Sets the currentPosition of the piece and calls BoardManager to record the move, destroy 
	/// any piece in the target position, and start the next turn. Use this after pieces have been
	/// initialized and the game has started
	/// </summary>
	/// <param name="position"></param>
	public void SetPosition(Vector2Int position)
	{
		currentPosition = position;
		boardManager.MovePiece(position, this);
		Debug.Log($"Piece's current position is now ({position.x}, {position.y})");
	}

	/// <summary>
	/// Returns the color of the piece
	/// </summary>
	/// <returns>PieceColor of the piece</returns>
	public PieceColor GetColor()
	{
		return color;
	}

	/// <summary>
	/// Gets the legal moves for the piece
	/// </summary>
	/// <returns>List of Vector2Int positions</returns>
	public abstract List<Vector2Int> GetMoves();

	/// <summary>
	/// Iterates through moves in a row according to the x-direction and y-direction parameters and checks 
	/// for friendly piece blocking, out of bounds moves, and returns all the legal moves
	/// </summary>
	/// <param name="xDirection">positive or negative x value</param>
	/// <param name="yDirection">positive or negative y value</param>
	/// <returns></returns>
	public List<Vector2Int> GetSlidingMoves(int xDirection, int yDirection)
	{
		List<Vector2Int> moves = new List<Vector2Int>();
		Vector2Int nextPos = currentPosition + new Vector2Int(xDirection, yDirection);

		while (boardManager.IsWithinBounds(nextPos))
		{
			if (boardManager.GetPieceAtPosition(nextPos) == null)
			{
				moves.Add(nextPos);
			}
			else if (boardManager.GetPieceAtPosition(nextPos).color != color)
			{
				moves.Add(nextPos);
				break;
			}
			else if (boardManager.GetPieceAtPosition(nextPos).color == color)
			{
				break;
			}
			nextPos += new Vector2Int(xDirection, yDirection);
		}
		return moves;
	}
}
