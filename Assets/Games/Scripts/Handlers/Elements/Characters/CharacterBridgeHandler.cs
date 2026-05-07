using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

namespace YunaSpace.BridgeRace
{
    public class CharacterBridgeHandler : MonoBehaviour
    {
        public Character Manager;

        private void OnTriggerEnter(Collider other)
        {            
            if (other.CompareTag("BridgePoint"))
            {
                var bridge = other.GetComponent<Bridge>();
                bridge.RefeshBoundary(Manager.ColorType, Manager is Player);

                if (Manager is Player && Manager.IsBuilding && Manager.BrickCount > 0)
                {
                    Manager.SetIsBuilding(false);
                }
            }

            if (other.CompareTag("BridgeStair"))
            {
                var stair = other.GetComponent<StairBrick>();

                OnBridgeStairTriggered(stair);
            }
        }

        private void OnBridgeStairTriggered(StairBrick stair)
        {
            Bridge bridge = stair.Bridge;

            if (stair.CanPlaceStair(Manager.ColorType) && Manager.BrickCount > 0)
            {
                stair.UpdateStair(true, Manager.ColorType);

                Manager.AddBrick(false);

                Manager.OnBrickCountChanged();

                bridge.RefeshBoundary(stair, Manager.ColorType, Manager is Player);
            }
            else if (stair.ColorType != Manager.ColorType && stair.ColorType != ColorType.None)
            {
                if (Vector3.Dot(Manager.transform.forward, Vector3.back) > 0.5f)
                {
                    bridge.RefeshBoundary(stair, Manager.ColorType, Manager is Player);
                }
            }
            else if (stair.ColorType == Manager.ColorType)
            {
                if (Vector3.Dot(Manager.transform.forward, Vector3.forward) > 0.5f)
                {
                    bridge.RefeshBoundary(stair, Manager.ColorType, Manager is Player);
                }
            }

            if (stair.IsFullBridge(Manager.ColorType))
            {
                Manager.OnFullBridgeReached(bridge.NextStage);

                if (Manager is Player)
                {
                    bridge.LockForPlayer();
                }
            }
        }
    }
}