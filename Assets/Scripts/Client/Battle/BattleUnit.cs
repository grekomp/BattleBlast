using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleBlast
{
	public class BattleUnit : MonoBehaviour, IPointerClickHandler
	{
		[Header("Battle Unit Settings")]
		public FloatReference movementAnimationSpeed = new FloatReference(1f);
		[Space]
		public GameObject selectionHighlight;
		public GameObject orderPreviewArrow;
		[Space]
		public TextMeshProUGUI attackText;
		public TextMeshProUGUI countText;


		[Space]
		public Image unitBox;
		public Color friendlyUnitColor;
		public Color enemyUnitColor;


		[Header("Runtime variables")]
		public string unitInstanceId;
		public BoardTile tile;
		public IntReference attack = new IntReference();
		public IntReference count = new IntReference();
		public string playerId;
		public BoolReference isFriendlyUnit = new BoolReference();

		protected Coroutine movementCoroutine;


		#region Initialize
		public void Initialize(UnitInstanceData unitInstanceData)
		{
			unitInstanceId = unitInstanceData.unitInstanceId;
			SetAttack(unitInstanceData.attack);
			SetCount(unitInstanceData.count);
			playerId = unitInstanceData.playerId;
			isFriendlyUnit.Value = playerId == NetClient.Instance.PlayerId;

			SetColor();

			tile = BattleController.Instance.board[unitInstanceData.x, unitInstanceData.y];
			transform.position = tile.centerTransform.position;
		}

		private void SetColor()
		{
			unitBox.color = isFriendlyUnit ? friendlyUnitColor : enemyUnitColor;
		}
		public void SetAttack(int attack)
		{
			this.attack.Value = attack;
			attackText.text = attack.ToString();
		}
		public void SetCount(int count)
		{
			this.count.Value = count;
			countText.text = count.ToString();
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
		public async Task HandleUnitActionMove(UnitActionMove unitAction)
		{
			if (movementCoroutine != null) StopCoroutine(movementCoroutine);
			tile = BattleController.Instance.board[unitAction.toX, unitAction.toY];
			movementCoroutine = StartCoroutine(AnimateMoveTo(tile.CenterPosition));
			await Task.Delay(100);
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
			HideOrderArrow();
		}
		public async Task HandleUnitActionAttack(UnitActionAttack unitAction)
		{
			await Task.Delay(100);
		}
		public async Task HandleUnitActionRetaliate(UnitActionRetaliate unitAction)
		{
			await Task.Delay(100);
		}
		public void HandleUnitActionSetState(UnitActionSetState unitAction)
		{
			attack.Value = unitAction.attack;
			count.Value = unitAction.count;
		}
		public async Task HandleUnitActionDie(UnitActionDie unitAction)
		{
			await Task.Delay(100);
			BattleController.Instance.spawnedUnits.Remove(this);
			Destroy(gameObject);
		}
		#endregion


		#region Order direction arrow
		public void ShowOrderArrow(int x, int y)
		{
			if (x > tile.x)
			{
				ShowOrderArrow(MoveDirection.Right);
				return;
			}
			if (x < tile.x)
			{
				ShowOrderArrow(MoveDirection.Left);
				return;
			}
			if (y > tile.y)
			{
				ShowOrderArrow(MoveDirection.Down);
				return;
			}
			if (y < tile.y)
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
