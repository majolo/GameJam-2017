using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using Improbable.Unity.Core;
using Assets.Gamelogic.Core;
using UnityEngine.SceneManagement;
using Improbable.Entity.Component;
using Improbable;

namespace Assets.Gamelogic.Player
{
    // This MonoBehaviour will be enabled on server-side workers
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class ActionFirer : MonoBehaviour
    {
		[Require] private PlayerActions.Reader PlayerActionsReader;
        [Require] private Health.Writer HealthWriter;

        private void Start()
        {
            // actionFirer = gameObject.GetComponent<ActionFirer>();
        }
        	
        private void OnEnable()
        {
            PlayerActionsReader.StabTriggered.Add(OnStab);
            PlayerActionsReader.OperateActionTriggered.Add(OnOperateAction);
			HealthWriter.CommandReceiver.OnTakeDamage.RegisterResponse(TakeDamage);
        }

        private void OnDisable()
        {
            PlayerActionsReader.StabTriggered.Remove(OnStab);
            PlayerActionsReader.OperateActionTriggered.Remove(OnOperateAction);
			HealthWriter.CommandReceiver.OnTakeDamage.DeregisterResponse();
        }

        private void OnStab(Stab stab)
        {   
//            Debug.LogWarning("Stab action fired!");
//            int newHealth = HealthWriter.Data.currentHealth - 250;
//            HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
			var touchMap = PlayerActionsReader.Data.touchMap;

			foreach(var item in touchMap)
			{
				if (item.Value) {
					AttackPlayer (item.Key);
//					newMap[other.gameObject.EntityId()] = false;
//					PlayerActionsWriter.Send(new PlayerActions.Update().SetTouchMap(newMap));
				}

			}



        }

        private void OnOperateAction(OperateAction operateAction)
        {   
//            Debug.LogWarning("Operate action fired!");
        }

		private DamageResponse TakeDamage(DamageRequest request, ICommandCallerInfo callerInfo)
		{
//			uint desiredDamage = request.amount;
//			uint dealtDamage = System.Math.Min(desiredDamage, HealthWriter.Data.currentHealth);
//
//			HealthWriter.Send(new Health.Update().SetCurrentHealth(HealthWriter.Data.currentHealth - dealtDamage));

			int newHealth = HealthWriter.Data.currentHealth - 1000;
			HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));

			return new DamageResponse(1000);
		}


		void AttackPlayer(EntityId playerEntityId) 
		{
			SpatialOS.Commands
				.SendCommand(HealthWriter, Health.Commands.TakeDamage.Descriptor, new DamageRequest(1000), playerEntityId)
				.OnSuccess(OnDamageRequestSuccess)
				.OnFailure(OnDamageRequestFailure);
		}

		void OnDamageRequestSuccess(DamageResponse response)
		{
			Debug.Log("Take damage command succeeded; dealt damage: " + response.dealtDamage);
		}

		void OnDamageRequestFailure(ICommandErrorDetails response)
		{
			Debug.LogError("Failed to send take damage command with error: " + response.ErrorMessage);
		}

        // public void AttemptToStab(Vector3 direction)
        // {
        // }
    }
}