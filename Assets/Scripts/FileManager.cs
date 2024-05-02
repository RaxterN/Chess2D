using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TerrainUtils;

/// <summary>
/// Handles saving and loading the game
/// </summary>
public class FileManager : MonoBehaviour
{
	private BoardManager boardManager;
	private string filepoop;

	private void Start()
	{
		boardManager = GetComponent<BoardManager>();
	}

	/// <summary>
	/// Serializes the game state into a csv format and saves the file as 'save.csv'
	/// </summary>
	public void SaveGame()
	{
		string poop = Path.Combine(Application.persistentDataPath, "save.csv");
		
		using (StreamWriter writer = new StreamWriter(poop))
		{
			string currentTurn;
			if (boardManager.GetCurrentTurn() == PieceColor.White)
			{
				currentTurn = "W";
			}
			else
			{
				currentTurn = "B";
			}

			for (int y = 7; y >= 0; y--) //iterate this way to get the file to be oriented the same as the board
			{
				for (int x = 7; x>= 0; x--)
				{
					Vector2Int position = new Vector2Int(x, y);
					Piece piece = boardManager.GetPieceAtPosition(position);

					if (piece == null)
					{
						writer.Write("empty,");
					}
					else
					{
						string type = piece.type.ToString();
						string color;
						if (piece.color == PieceColor.White)
						{
							color = "W";
						}
						else
						{
							color = "B";
						}
						writer.Write($"{color} {type},");
					}
				}
				writer.Write("\n");
			}
			writer.Write($"{currentTurn}\n");
		}
		Debug.Log("Game has been saved.");
	} //to view this file yourself go to users/user/appdata/LocalLow/(company name)/Chess2D
	  //the default company name is DefaultCompany

	/// <summary>
	/// This method handles loading the game from the save file***************** to implement
	/// </summary>
	public void LoadGame()
	{
	}
}