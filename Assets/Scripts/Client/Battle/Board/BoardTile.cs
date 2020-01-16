using UnityEngine;
using UnityEngine.EventSystems;


namespace BattleBlast
{
	[RequireComponent(typeof(RectTransform))]
	public class BoardTile : DisposableMonoBehaviour, IPointerClickHandler
	{
		[Header("Board tile settings")]
		public RectTransform centerTransform;

		[Header("Runtime variables")]
		public int x;
		public int y;

		public RectTransform rectTransform;

		public Vector3 CenterPosition => centerTransform.position;



		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}

		#region Handling input
		public void OnPointerClick(PointerEventData eventData)
		{
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					BattleController.Instance.HandleBoardTileLeftClicked(this);
					break;
				case PointerEventData.InputButton.Right:
					BattleController.Instance.HandleBoardTileRightClicked(this);
					break;
			}
		}
		#endregion
	}
}