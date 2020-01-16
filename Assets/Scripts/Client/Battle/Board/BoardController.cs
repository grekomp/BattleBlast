using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleBlast
{
	[RequireComponent(typeof(RectTransform))]
	public class BoardController : ValueSetter
	{
		[Header("Board options")]
		[HandleChanges] public IntReference boardCellsX = new IntReference(20);
		[HandleChanges] public IntReference boardCellsY = new IntReference(20);
		[Space]
		[HandleChanges] public IntReference tileSpacing = new IntReference(10);
		[Space]
		[HandleChanges] public BoardTile tilePrefab;

		[Header("Runtime variables")]
		[SerializeField] protected List<BoardTile> tiles = new List<BoardTile>();
		public RectTransform rectTransform;


		#region Initialization
		protected override void Init()
		{
			rectTransform = GetComponent<RectTransform>();
		}
		#endregion


		#region Generating tiles
		protected override void ApplySet()
		{
			RemoveAllTiles();

			float tileWidth = (rectTransform.rect.width - ((boardCellsX - 1) * tileSpacing)) / boardCellsX;
			float tileHeight = (rectTransform.rect.height - ((boardCellsY - 1) * tileSpacing)) / boardCellsY;


			for (int i = 0; i < boardCellsX; i++)
			{
				for (int j = 0; j < boardCellsY; j++)
				{
					BoardTile tile = Instantiate(tilePrefab, transform);
					tile.x = i;
					tile.y = j;
					tile.rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
					tile.rectTransform.anchoredPosition = new Vector3(i * (tileWidth + tileSpacing), -j * (tileHeight + tileSpacing), 0);

					tiles.Add(tile);
				}
			}
		}

		protected void RemoveAllTiles()
		{
			foreach (var tile in tiles)
			{
				tile?.DisposeAndDestroy();
			}
			tiles.Clear();
		}
		#endregion


		#region Accessing tiles
		public BoardTile this[int x, int y] => tiles.Find(t => t.x == x && t.y == y);
		#endregion
	}
}
