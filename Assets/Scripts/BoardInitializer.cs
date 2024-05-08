using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Contains methods for initializing the board and the pieces into their default positions
/// </summary>
public class BoardInitializer : MonoBehaviour
{
	private BoardManager boardManager;

	//the list of start positions for each piece
	public List<StartPosition> StartPositions;

	//colors for the tiles - adjust in editor
	public Color whiteSquaresColor = new Color(208f/255f,208f/255f, 208f/255f, 1f);
	public Color blackSquaresColor = new Color(173f/255f, 216f/255f, 230f/255f, 1f);

	//piece prefabs
	public GameObject whitePawnPrefab, whiteRookPrefab, whiteKnightPrefab, whiteBishopPrefab, whiteQueenPrefab, whiteKingPrefab;
	public GameObject blackPawnPrefab, blackRookPrefab, blackKnightPrefab, blackBishopPrefab, blackQueenPrefab, blackKingPrefab;

	//tile prefab
	public GameObject squarePrefab;

	// Start is called before the first frame update
	void Start()
	{
		//Get BoardManager reference
		boardManager = GetComponent<BoardManager>();
		if (boardManager == null)
		{
			Debug.LogError("BoardManager not instantiated or not captured by BoardInitializer");
			return;
		}

		//Creates and positions the board tiles
		InitializeTiles();
		InitializePieces();

		//Log the board state after intialization
		Debug.Log("Board Initialized: \n");
		boardManager.PrintGameState();
	}

	/// <summary>
	/// Instantiates the tiles composing the game board
	/// </summary>
	public void InitializeTiles()
	{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					GameObject square = Instantiate(squarePrefab, new Vector2(i, j), Quaternion.identity);
					square.transform.parent = this.transform;

					//this determines what squares should be what color
					if ((i + j + 1) % 2 == 0)
					{
						square.GetComponent<SpriteRenderer>().color = whiteSquaresColor;
					}
					else
					{
						square.GetComponent<SpriteRenderer>().color = blackSquaresColor;
					}
					//Debug.Log("Creating square at: " + i + ", " + j);
				}
			}
	}

	/// <summary>
	/// Spawns the pieces in their proper places
	/// </summary>
	public void InitializePieces()
	{
		foreach (StartPosition sp in StartPositions)
		{
			foreach(Vector2Int position in sp.positions)
			{
				//Make a new Vector3 for instantiation b/c Unity only instantiates with Vector3's
				Vector3 spawnPos = new Vector3(position.x, position.y, 0);

				/* Instantiate the prefab and get the Piece script from it.
				 * Then set the position in code of each one to the physical position 
				   This works because each script (Rook, Pawn, King etc.) are inheriting from Piece and unity gets subclasses of Piece*/
				GameObject pieceObject = Instantiate(sp.prefab, spawnPos, Quaternion.identity); 
				Piece pieceScript = pieceObject.GetComponent<Piece>();
				pieceScript.SetPositionOnly(position);
			}
		}
	}
}
