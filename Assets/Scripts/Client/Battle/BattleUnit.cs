using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleBlast
{
	public class BattleUnit : MonoBehaviour, IPointerClickHandler
	{
		[Header("Battle Unit Settings")]
		public FloatReference movementAnimationSpeed = new FloatReference(1f);
		[Space]
		public GameObject selectionHighlight;
		public GameObject orderPreviewArrow;

		[Header("Runtime variables")]
		public string unitInstanceId;
		public BoardTile tile;
		public IntReference attack = new IntReference();
		public IntReference count = new IntReference();
		public string playerId;


		protected Coroutine movementCoroutine;


		#region Initialize
		public void Initialize(UnitInstanceData unitInstanceData)
		{
			unitInstanceId = unitInstanceData.unitInstanceId;
			attack.Value = unitInstanceData.attack;
			count.Value = unitInstanceData.count;

			tile = BattleController.Instance.board[unitInstanceData.x, unitInstanceData.y];
			transform.position = tile.centerTransform.position;
		}
		#endregion


		#region Selection
		public void OnPointerClick(PointerEventData eventData)
		{
			switch (eventData.button)
			{
				case PointerEventData.InputButton.Left:
					BattleController.Instance.HandleBattleUnitLeftClicked(this);
					break;
				case PointerEventData.InputButton.Right:
					BattleController.Instance.HandleBattleUnitRightClicked(this);
					break;
			}
		}

		public void Select()
		{
			selectionHighlight?.SetActive(true);
		}
		public void Deselect()
		{
			selectionHighlight?.SetActive(false);
		}
		#endregion


		#region Handling unit actions
		#region Handling movement
		public void HandleUnitActionMove(UnitActionMove unitAction)
		{
			if (movementCoroutine != null) StopCoroutine(movementCoroutine);
			tile = BattleController.Instance.board[unitAction.toX, unitAction.toY];
			movementCoroutine = StartCoroutine(AnimateMoveTo(tile.CenterPosition));
		}

		protected IEnumerator AnimateMoveTo(Vector3 targetPosition)
		{
			while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementAnimationSpeed);
				yield return null;
			}

			transform.position = targetPosition;
		}
		#endregion

		public void HandleUnitActionStop(UnitActionStop unitAction)
		{

		}
		public void HandleUnitActionAttack(UnitActionAttack unitAction)
		{

		}
		public void HandleUnitActionRetaliate(UnitActionRetaliate unitAction)
		{

		}
		public void HandleUnitActionSetState(UnitActionSetState unitAction)
		{
			attack.Value = unitAction.attack;
			count.Value = unitAction.count;
		}
		#endregion


		#region Order direction arrow
		public void ShowOrderArrow(BoardTile target)
		{
			if (target.x > tile.x)
			{
				ShowOrderArrow(MoveDirection.Right);
				return;
			}
			if (target.x < tile.x)
			{
				ShowOrderArrow(MoveDirection.Left);
				return;
			}
			if (target.y > tile.y)
			{
				ShowOrderArrow(MoveDirection.Down);
				return;
			}
			if (target.y < tile.y)
			{
				ShowOrderArrow(MoveDirection.Up);
				return;
			}

			HideOrderArrow();
		}
		public void ShowOrderArrow(MoveDirection direction)
		{
			float zRotation = 0;
			switch (direction)
			{
				case MoveDirection.Left:
					zRotation = -90;
					break;
				case MoveDirection.Up:
					zRotation = 180;
					break;
				case MoveDirection.Right:
					zRotation = 90;
					break;
				case MoveDirection.Down:
					zRotation = 0;
					break;
				case MoveDirection.None:
					HideOrderArrow();
					return;
			}

			orderPreviewArrow.transform.rotation = Quaternion.Euler(0, 0, zRotation);
			orderPreviewArrow?.SetActive(true);
		}
		public void HideOrderArrow()
		{
			orderPreviewArrow?.SetActive(false);
		}
		#endregion
	}
}
