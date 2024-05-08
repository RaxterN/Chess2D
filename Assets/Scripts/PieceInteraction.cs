using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles all user interaction with piece gameobjects on the board
/// </summary>
public class PieceInteraction : MonoBehaviour
{
	private BoardManager boardManager;
	private Piece pieceScript;
	private Vector3 screenPoint;
	private Vector3 offset;
	private List<GameObject> currentMoveSquares = new List<GameObject>();
	public GameObject moveSquare;
	private Vector3 dropPosition;
	List<Vector2Int> validMoves = new List<Vector2Int>();
	private Vector3 originalPosition;

	// Start is called before the first frame update
	void Start()
	{
		//get boardManager reference on piece instantiation 
		boardManager = FindObjectOfType<BoardManager>();
		//get reference to the piece script
		pieceScript = GetComponent<Piece>();
	}

	/// <summary>
	/// This is called whenever the mouse is clicked over a collider for a piece
	/// </summary>
	void OnMouseDown()
	{
		if (pieceScript.GetColor() == boardManager.GetCurrentTurn())
		{
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
			originalPosition = transform.position;

			//Get a list of the valid moves for the piece & spawn move indicators at those positions
			validMoves.Clear();
			validMoves = pieceScript.GetMoves();
			SpawnMoveSquares(validMoves);
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// This is called while the mouse stays held down
	/// </summary>
	void OnMouseDrag()
	{
		if (pieceScript.color == boardManager.GetCurrentTurn())
		{
			Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
			transform.position = cursorPosition;

			//while holding a piece, calculate the square it would land on
			dropPosition = SnapToGrid(transform.position);
		}
	}

	/// <summary>
	/// This is called when the mouse button is released
	/// </summary>
	void OnMouseUp()
	{
		if (pieceScript.color == boardManager.GetCurrentTurn())
		{
			//get the potential move in Vector2Int
			Vector2Int potentialMove = new Vector2Int(Mathf.RoundToInt(dropPosition.x), Mathf.RoundToInt(dropPosition.y));
			if (validMoves.Contains(potentialMove))
			{
				//snap the piece to the closest grid position
				transform.position = SnapToGrid(transform.position);

				Vector2Int v2pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
				pieceScript.SetPosition(v2pos); //make the move in the piece script, calling BoardManager from there

				DestroyMoveSquares();
			}
			else
			{
				transform.position = originalPosition;
				DestroyMoveSquares();
			}
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// Takes a vector3 and rounds it to the nearest grid position
	/// </summary>
	/// <param name="position">The initial position</param>
	/// <returns>Vector3 with values rounded to nearest integer</returns>
	Vector3 SnapToGrid(Vector3 position)
	{
		float x = Mathf.Round(position.x);
		float y = Mathf.Round(position.y);
		return new Vector3(x, y, 0);
	}

	/// <summary>
	/// Takes the list of legal moves for the selected piece and spawns tiles to indicate possible moves
	/// </summary>
	/// <param name="validMoves">List of possible moves</param>
	public void SpawnMoveSquares(List<Vector2Int> validMoves)
	{
		foreach (Vector2Int move in validMoves)
		{
			Vector3 v3position = new Vector3(move.x, move.y, 0);
			currentMoveSquares.Add(Instantiate(moveSquare, v3position, Quaternion.identity));
		}
	}

	/// <summary>
	/// Destroys the currently instantiated move tiles 
	/// </summary>
	public void DestroyMoveSquares()
	{
		foreach (GameObject tile in currentMoveSquares)
		{
			Destroy(tile);
		}
		currentMoveSquares.Clear();
	}

	/// <summary>
	/// Moves the piece gameobject to a specific location. Use this for when the boardManager requires a piece to 
	/// move without being selected, such as during castling 
	/// </summary>
	/// <param name="newPosition">World position to move to</param>
	public void MoveTo(Vector3 newPosition)
	{
		transform.position = newPosition;
	}
}


	/* CITATIONS:     ******************************************************************
	 * 1. ChatGPT introduced me to the OnMouseDown() etc. methods and how all that works
	 *		https://chat.openai.com/share/4beb61a2-6250-4b4b-a926-039cd1ead597
	 *		More about ScreenToWorldPoint: https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html
	 *		And also the collision methods for monobehavior scripts can be found here: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	 *		*MonoBehaviour is the class you inherit from in your C# scripts in unity so you can use all these useful methods
	 */
