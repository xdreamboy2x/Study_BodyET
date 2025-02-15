﻿//using ETModel;
//using PF;
//using Vector3 = UnityEngine.Vector3;
//using UnityEngine;
//using System.Collections.Generic;

//namespace ETHotfix
//{
//	[MessageHandler]
//	public class M2C_OnEnterViewHandler : AMHandler<M2C_OnEnterView>
//	{
//		protected override async ETTask Run(ETModel.Session session, M2C_OnEnterView message)
//		{
//			var entity = await OnEnterView(message.EnterEntity);
//			if (entity is Bullet bullet)
//			{
//				var pos = new Vector3(message.X / 100f, message.Y / 100f, message.Z / 100f);
//				bullet.BodyView = GameObject.Instantiate(PrefabHelper.GetUnitPrefab("BulletSmallBlue"), pos, Quaternion.identity);
//				bullet.BodyView.name = $"Bullet#{bullet.Id}";
//				bullet.TransformComponent.transform = bullet.BodyView.transform;
//				bullet.TransformComponent.SetPosition(pos);
//				Log.Debug($"bullet {pos}");
//			}
//			await ETTask.CompletedTask;
//		}
		
//		public static async ETTask<ETModel.Entity> OnEnterView(EntiyInfo entityInfo)
//		{
//			try
//			{
//				Log.Debug($"{EntityDefine.GetType(entityInfo.Type).Name}");
//				if (entityInfo.Type == EntityDefine.GetTypeId<Unit>())
//				{
//					var remoteUnit = MongoHelper.FromBson<Unit>(entityInfo.BsonBytes.bytes);
//					foreach (var item in remoteUnit.Components)
//						Log.Debug($"remoteUnit {item.GetType().Name}");
//					//remoteUnit.Domain = ETModel.Game.Scene;
//					//Unit unit = UnitFactory.Create(ETModel.Game.Scene, remoteUnit.Id);
//					var go = UnityEngine.Object.Instantiate(PrefabHelper.GetUnitPrefab("RemoteUnit"));
//					go.transform.position = remoteUnit.Position;
//					GameObject.DontDestroyOnLoad(go);
//					//var unit = EntityFactory.CreateWithId<Unit>(domain, id);
//					ETModel.Game.EventSystem.RegisterSystem(remoteUnit);
//					ETModel.Game.EventSystem.Awake(remoteUnit);
//					remoteUnit.Awake(go);
//					UnitComponent.Instance.Add(remoteUnit);
//					remoteUnit.Position = remoteUnit.Position;
//					//remoteUnit.Dispose();
//					return remoteUnit;
//				}
//				if (entityInfo.Type == EntityDefine.GetTypeId<Bullet>())
//				{
//					var remoteBullet = MongoHelper.FromBson<Bullet>(entityInfo.BsonBytes.bytes);
//					//Log.Debug($"{remoteBullet}");
//					//var bullet = ETModel.EntityFactory.CreateWithId<Bullet>(ETModel.Game.Scene, remoteBullet.Id);
//					Log.Debug($"{remoteBullet.Components.Count}");
//					ETModel.Game.EventSystem.RegisterSystem(remoteBullet);
//					ETModel.Game.EventSystem.Awake(remoteBullet);
//					BulletComponent.Instance.Add(remoteBullet);
//					//remoteBullet.Dispose();
//					return remoteBullet;
//				}
//				if (entityInfo.Type == EntityDefine.GetTypeId<Monster>())
//				{
//					var remote = MongoHelper.FromBson<Monster>(entityInfo.BsonBytes.bytes);
//					Log.Debug($"HealthComponent HP{remote.GetComponent<HealthComponent>().HP}");
//					remote.Awake();
//					remote.Domain = ETModel.Game.Scene;
//					remote.BodyView = GameObject.Instantiate(PrefabHelper.GetUnitPrefab("Monster"));
//					GameObject.DontDestroyOnLoad(remote.BodyView);
//					//var monster = MonsterFactory.Create(ETModel.Game.Scene, remote.Id);
//					MonsterComponent.Instance.Add(remote);
//					//remote.Position = remote.Position;
//					//remote.Dispose();
//					return remote;
//				}
//			}
//			catch (System.Exception e)
//			{
//				Log.Error(e);
//			}
//			return null;
//		}
//    }
//}
