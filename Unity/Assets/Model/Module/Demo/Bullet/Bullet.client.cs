﻿using UnityEngine;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
	public partial class Bullet
	{
		public GameObject BodyView { get; set; }


		public override void Dispose()
		{
			if (BodyView != null)
				GameObject.Destroy(BodyView);
			BodyView = null;
			base.Dispose();
		}
	}
}