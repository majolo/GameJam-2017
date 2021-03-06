﻿using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor 
{
	public class SnapshotMenu : MonoBehaviour
	{
		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		[UsedImplicitly]
		private static void GenerateDefaultSnapshot()
		{
			var snapshotEntities = new Dictionary<EntityId, Entity>();
			var currentEntityId = 1;

			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateBananaCreatorTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateLaboratoryTemplate());
//			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateBananaTemplate());
//			PopulateSnapshotWithBananaEntities(ref snapshotEntities, ref currentEntityId);

			SaveSnapshot(snapshotEntities);
		}

		public static void PopulateSnapshotWithBananaEntities(ref Dictionary<EntityId, Entity> snapshotEntities, ref int nextAvailableId)
		{	

			for (var i = 0; i < 20; i++)
			{
				// Choose a starting position for this banana entity
				var x = Random.Range(-35,35);
				var z = Random.Range(-35,35);
				var bananaCoordinates = new Vector3(x,0.3f,z);


				snapshotEntities.Add(new EntityId(nextAvailableId++),
					EntityTemplateFactory.CreateBananaTemplate(bananaCoordinates));
			}
		}

		private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
		{
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

			if (maybeError.HasValue)
			{
				Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
			}
			else
			{
				Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
			}
		}
	}
}
