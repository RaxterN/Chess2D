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
	public void CommitMove(Vector2Int newPosition, Piece piece)
	{
		//Record the move
		Move move = new Move(newPosition, piece.currentPosition, piece);
		moves.Push(move);

		//Handle en passant moves
		if (IsEnPassant(piece, newPosition))
		{
			Vector2Int enPassantCapturedPosition = new Vector2Int(newPosition.x, piece.currentPosition.y);
			DestroyPieceAtPosition(enPassantCapturedPosition);
		}

		//Handle castling moves
		if (piece.type == PieceType.King && Math.Abs(newPosition.x - piece.currentPosition.x) == 2)//a king moving two squares will always be a castle
		{
			//Castling attempt, determine if kingside or queenside
			bool isKingside = newPosition.x > piece.currentPosition.x;
			Vector2Int rookOriginalPosition = isKingside ? new Vector2Int(7, newPosition.y) : new Vector2Int(0, newPosition.y);
			Vector2Int rookNewPosition = isKingside ? new Vector2Int(5, newPosition.y) : new Vector2Int(3, newPosition.y);

			//Move the rook
			Piece rook = GetPieceAtPosition(rookOriginalPosition);                      
			Vector3 newPhysicalPosition = BoardToScenePosition(rookNewPosition);
			rook.GetComponent<PieceInteraction>().MoveTo(newPhysicalPosition);

			//Update the board state to reflect rook moving
			gameBoard.ClearPosition(rook.currentPosition.x, rook.currentPosition.y);
			gameBoard.SetPosition(rook, rookNewPosition.x, rookNewPosition.y);
			rook.currentPosition = rookNewPosition;
			rook.hasMoved = true;
		}

		//clear the original position of the piece
		gameBoard.ClearPosition(piece.currentPosition.x, piece.currentPosition.y);

		//destroy any piece at the target position
		DestroyPieceAtPosition(newPosition);

		//clear and set target position with new piece
		gameBoard.ClearPosition(newPosition.x, newPosition.y);
		gameBoard.SetPosition(piece, newPosition.x, newPosition.y);

		NextTurn();
		PrintGameState();//debug log the state
	}

	/// <summary>
	/// Only sets the position of a piece in the gameBoard without starting the 
	/// next turn or recording the move. Use this for initializing the board, during castling etc.
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
	/// Destroys the gameobject associated with a piece at a certain position in the gameBoard
	/// </summary>
	/// <param name="targetPostion">Position to clear</param>
	public void DestroyPieceAtPosition(Vector2Int targetPostion)
	{
		int x = targetPostion.x;
		int y = targetPostion.y;
		Piece targetPiece = gameBoard.GetPieceAtPosition(x, y);
		if (targetPiece != null)
		{
			Destroy(targetPiece.gameObject);
		}
		return;
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
	/// Returns the current turn count
	/// </summary>
	/// <returns>Int</returns>
	public int GetTurnCount()
	{
		return turnCount;
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
	/// Checks whether a move was a double pawn move
	/// </summary>
	/// <param name="move">Move to check</param>
	/// <returns>Bool</returns>
	public bool IsDoublePawnMove(Move move)
	{
		if (move.piece.type == PieceType.Pawn)
		{
			if (Math.Abs(move.startPosition.y - move.endPosition.y) == 2)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the current move operation is an en passant move. Note that this does not check a Move, 
	/// but rather checks the values associated with moving a piece
	/// </summary>
	/// <param name="piece">The piece moving</param>
	/// <param name="newPostion">The new position of the piece</param>
	/// <returns></returns>
	public bool IsEnPassant(Piece piece, Vector2Int newPostion)
	{
		if (piece.type == PieceType.Pawn)//must be a pawn moving
		{
			if (GetPieceAtPosition(newPostion) == null)//target position must be empty
			{
				if (piece.currentPosition.x != newPostion.x)//pawn must be capturing to the side
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// COnverts a Vector2Int board position into a Vector3 world position
	/// </summary>
	/// <param name="boardPosition">Position of the piece of the board</param>
	/// <returns>Vector3</returns>
	private Vector3 BoardToScenePosition(Vector2Int boardPosition)
	{
		// Implementation needed to translate board coordinates to Unity world coordinates
		return new Vector3(boardPosition.x, boardPosition.y);
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
*/