using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CharacterShoveHandler : MonoBehaviour
    {
        public Character Manager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Character"))
            {
                if (Manager.IsBuilding)
                {
                    return;
                }

                var character = other.GetComponent<CharacterShoveHandler>().Manager;

                OnCharacterShoved(character);
            }
        }

        private void OnCharacterShoved(Character character)
        {
            if (Manager.BrickCount != character.BrickCount)
            {
                var shoverer = Manager.BrickCount > character.BrickCount ? Manager : character;
                var stumbler = Manager.BrickCount < character.BrickCount ? Manager : character;

                if (stumbler.BrickCount > 0 && stumbler.IsStumbling == false)
                {
                    Vector3 direction = (stumbler.transform.position - shoverer.transform.position).normalized;

                    stumbler.Stumble(direction);

                    if (shoverer is Enemy enemy)
                    {
                        enemy.ChangeState<CollectState>();
                    }
                }
            }
        }
    }
}