using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This class manages that game state, handling piece movement, checking positions, turns, and other data 
/// needed for the chess game. It contains an instance of the Board class that holds a representation of the 
/// board as a 2D array. Only one BoardManager should exist at a time.
/// </summary>
public class BoardManager : MonoBehaviour
{
    private Board gameBoard;
	private GameObject mainCamera;
	private BoardInitializer boardInitializer;
    private int turnCount = 0;
	private PieceColor currentTurn;
	private bool isCheckmate;
	private bool isCheck;
	private bool isStalemate;
	private bool isDraw;
	public Stack<Move> moves = new Stack<Move>();

	private void Awake()
	{
		//This must happen in Awake() or the BoardInitializer can't capture the reference on Start()
		gameBoard = new Board();
		currentTurn = PieceColor.White;
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	private void Start()
	{
		boardInitializer = GetComponent<BoardInitializer>();
	}

	/// <summary>
	/// Moves the recorded location of a piece in the gameBoard, clears the target position, 
	/// and starts the next turn
	/// </summary>
	/// <param name="newPosition">The position the piece is moving to</param>
	/// <param name="piece">The piece</param>
	public void MovePiece(Vector2Int newPosition, Piece piece)
	{
		//Record the move
		Move move = new Move(newPosition, piece.currentPosition, piece);
		moves.Push(move);

		//clear the original position of the piece
		gameBoard.ClearPosition(piece.currentPosition.x, piece.currentPosition.y);

		//destroy any piece at the target position
		int x = newPosition.x;
		int y = newPosition.y;
		Piece targetPiece = gameBoard.GetPieceAtPosition(x, y);
		if (targetPiece != null)
		{
			Destroy(targetPiece.gameObject);
		}

		//clear and set target position with new piece
		gameBoard.ClearPosition(x, y);
		gameBoard.SetPosition(piece, x, y);

		NextTurn();
		PrintGameState();//debug log the state
	}

	/// <summary>
	/// Only sets the position of a piece in the gameBoard without starting the 
	/// next turn or recording the move. Use this for initializing the board
	/// </summary>
	/// <param name="position">Position of piece</param>
	/// <param name="piece">Piece</param>
	public void PlacePiece(Vector2Int position, Piece piece)
	{
		int x = position.x;
		int y = position.y;

		gameBoard.SetPosition(piece, x, y);
	}

	/// <summary>
	/// Returns the piece at a specific position on the game board
	/// </summary>
	/// <param name="position">The position to check</param>
	/// <returns>Piece</returns>
	public Piece GetPieceAtPosition(Vector2Int position)
	{
		int x = position.x;
		int y = position.y;
		//Debug.Log($"BoardManager is checking for piece at ({x},{y}");
		Piece piece = gameBoard.GetPieceAtPosition(x, y);
		return piece;
	}

	/// <summary>
	/// This method will call when a valid move is made and flip the board and set the appropriate variable
	/// </summary>
	public void NextTurn()
	{
		if (currentTurn == PieceColor.White)
		{
			currentTurn = PieceColor.Black;
			mainCamera.transform.rotation = Quaternion.Euler(0, 0, 180);
		}
		else
		{
			currentTurn = PieceColor.White;
			mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		RotatePieces(GetAllPieceObjects()); //rotate all pieces + the camera each turn
		turnCount++;
		Debug.Log($"Next turn started. It is now {currentTurn}'s turn.");
	}

	/// <summary>
	/// Returns the color of the current player's turn
	/// </summary>
	/// <returns>PieceColor</returns>
	public PieceColor GetColorTurn()
	{
		return currentTurn;
	}

	/// <summary>
	/// Checks the gameBoard to see if the move is within bounds
	/// </summary>
	/// <param name="move">The move location</param>
	/// <returns>bool</returns>
	public bool IsWithinBounds(Vector2Int move)
	{
		int x = move.x;
		int y = move.y;
		return gameBoard.IsMoveWithinBounds(x, y);
	}

	/// <summary>
	/// Gets all current pieces on the board and returns the game objects they are attached to 
	/// </summary>
	/// <returns>List of GameObjects</returns>
	public List<GameObject> GetAllPieceObjects()
	{
		List<GameObject> currentPieces = new List<GameObject>();

		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				Vector2Int positon = new Vector2Int(x, y);
				Piece piece = GetPieceAtPosition(positon);
				if (piece != null)
				{
					GameObject pieceObject = piece.gameObject; //get the actual gameobject the Piece script is on 
					currentPieces.Add(pieceObject);
				}
			}
		}
		return currentPieces;
	}

	/// <summary>
	/// Clears all the recorded positions in the gameBoard
	/// </summary>
	public void ClearBoardPositions()
	{
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				gameBoard.ClearPosition(x, y);
			}
		}
	}

	/// <summary>
	/// Takes a list of GameObjects consisting of the current pieces and rotates them 180 degrees
	/// </summary>
	/// <param name="pieceObjects">List of current piece gameobjects on the board</param>
	public void RotatePieces(List<GameObject> pieceObjects)
	{
		foreach (GameObject pieceObject in pieceObjects)
		{
			Quaternion rotation = pieceObject.transform.rotation;
			Quaternion newRotation = Quaternion.Euler(0, 0, 180) * rotation;
			pieceObject.transform.rotation = newRotation;
		}
	}

	/// <summary>
	/// Clears the gameBoard, destroys the piece prefabs, and re-initializes them
	/// </summary>
	public void NewGame()
	{
		List<GameObject> currentPieces = GetAllPieceObjects();

		for (int i = 0; i < currentPieces.Count; i++)
		{
			Destroy(currentPieces[i]);
		}

		//clear the gameBoard, instantiate prefabs, rotate camera back, and reset it to white's turn
		ClearBoardPositions();
		boardInitializer.InitializePieces();
		mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
		currentTurn = PieceColor.White;

		Debug.Log("New game started: \n");
		PrintGameState();
	}

	/// <summary>
	/// Returns the PieceColor associated with the current turn (currentTurn)
	/// </summary>
	/// <returns>PieceColor</returns>
	public PieceColor GetCurrentTurn()
	{
		return currentTurn;
	}

	/// <summary>
	/// Prints the game state to console
	/// </summary>
	public void PrintGameState()
	{
		//store the row strings in array iterate backwards to avoid printing in reverse order
		string[] rows = new string[8];
		for (int y = 0; y < 8; y++)
		{
			string row = "\t\t\t";
			for (int x = 0; x < 8; x++)
			{
				if (gameBoard.board[x, y] == null)
				{
					row += "Empty\t";
				}
				else
				{
					row += gameBoard.board[x, y].GetType().Name + "\t";
				}
			}
			rows[y] = row;
		}
		for (int i = 7; i>=0; i--)
		{
			Debug.Log(rows[i]);
		}
	}
}


/* CITATIONS: ***********************************************************************************************
 * 
 * https://docs.unity3d.com/Manual/Tags.html for how to find objects in the scene using tags
 * The camera in unity seems to be pre-tagged as MainCamera
 * 
 * https://chat.openai.com/share/2d0bf783-15be-4885-83e9-85c8eec60235 for how I made the RotatePieces method
 */