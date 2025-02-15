﻿using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
    {
	    public override void Awake(OperaComponent self)
	    {
		    self.Awake();
	    }
    }

	[ObjectSystem]
	public class OperaComponentUpdateSystem : UpdateSystem<OperaComponent>
	{
		public override void Update(OperaComponent self)
		{
			self.Update();
		}
	}

	public class OperaComponent: Entity
    {
        public Vector3 ClickPoint;
	    public int MapMask { get; set; }
		private Vector3 _lastDirection { get; set; }

	    public void Awake()
	    {
		    this.MapMask = LayerMask.GetMask("Map");
			_lastDirection = Vector3.zero;
		}

	    private readonly UnitOperation msg = new UnitOperation();
		private long lastSendTime;
		public void Update()
        {
			var localUnit = Unit.LocalUnit;
			if (localUnit == null)
				return;
			if (localUnit.BodyView == null)
				return;
			if (localUnit.SkillDiretorTrm != null)
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000, this.MapMask))
				{
					localUnit.SkillDiretorTrm.position = localUnit.Position;
					var trmP = localUnit.SkillDiretorTrm.position;
					var direction = new Vector3(hit.point.x, trmP.y, hit.point.z) - trmP;
					//var dist = Vector3.Distance(direction, _lastDirection);
					localUnit.SkillDiretorTrm.forward = _lastDirection = direction;
				}
			}

			msg.UnitId = localUnit.Id;
			msg.Index++;
			msg.Operation = 0;
			var p = localUnit.Position;
			msg.X = (int)(p.x * 100);
			msg.Y = (int)(p.y * 100);
			msg.Z = (int)(p.z * 100);
			msg.AngleY = (int)(localUnit.Rotation.y * 100);
			if (Input.GetMouseButtonDown(0))
			{
				localUnit.Firing = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				localUnit.Firing = false;
			}
			if (localUnit.Firing)
			{
				if (TimeHelper.Now() - localUnit.LastFireTime < 200)
					return;
				if (localUnit.SkillDiretorTrm)
				{
					localUnit.LastFireTime = TimeHelper.Now();

					//if (!localUnit.PreviousFiring)
					//{
					//	localUnit.PreviousFiring = true;
					//	localUnit.CharacterController.MaxStableMoveSpeed = 4;
					//	localUnit.CharacterController.LockRotation(localUnit.SkillDiretorTrm.localEulerAngles);
					//}
					//localUnit.CharacterController.SetRotation(localUnit.SkillDiretorTrm.localEulerAngles);

					msg.Operation = OperaType.Fire;
					msg.AngleY = (int)(localUnit.Rotation.y * 100);
					p = localUnit.SkillDiretorTrm.Find("TargetPoint").position;
					msg.IntParams.Clear();
					msg.LongParams.Clear();
					var x = (int)(p.x * 100);
					var y = (int)(p.y * 100);
					var z = (int)(p.z * 100);
					msg.IntParams.Add(x);
					msg.IntParams.Add(y);
					msg.IntParams.Add(z);
					var bulletId = IdGenerater.GenerateId();
					msg.IntParams.Add(1);
					msg.LongParams.Add(bulletId);
					SessionHelper.HotfixSend(msg);
				}
				return;
			}
			else
			{
				//if (localUnit.PreviousFiring)
				//{
				//	localUnit.PreviousFiring = false;
				//	await TimerComponent.Instance.WaitAsync(100);
				//	localUnit.CharacterController.MaxStableMoveSpeed = 10;
				//	localUnit.CharacterController.CancelLockRotation();
				//}
			}

			if (TimeHelper.Now() - lastSendTime > 100)
			{
				lastSendTime = TimeHelper.Now();
				if (Vector3.Distance(localUnit.LastPosition, p) < 0.05f)
					return;
				localUnit.LastPosition = p;
				SessionHelper.HotfixSend(msg);
			}
		}
    }
}