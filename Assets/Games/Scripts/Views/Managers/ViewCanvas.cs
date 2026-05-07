using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class ViewCanvas : MonoBehaviour
    {
        public virtual void OnOpened()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void OnClosed()
        {
            this.gameObject.SetActive(false);
        }
    }

    public class ViewCanvas<T> : ViewCanvas where T : ViewCanvas<T>
    {
        public void Close() => View.CloseCanvas<T>();
    }
}
