using TMPro;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CharacterNameBadge : MonoBehaviour
    {
        [SerializeField] private TextMeshPro nameBadge;

        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            nameBadge.transform.rotation = cameraTransform.rotation;
        }
    }
}