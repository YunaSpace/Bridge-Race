using System;
using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class ViewManager : Singleton<ViewManager>
    {
        [SerializeField] private List<ViewCanvas> canvasList = new();

        private Dictionary<Type, ViewCanvas> canvases = new();

        protected override void Awake()
        {
            base.Awake();

            GetAllCanvas();
        }

        public T OpenCanvas<T>(bool closeOtherCanvas = false) where T : ViewCanvas
        {
            if (canvases.TryGetValue(typeof(T), out ViewCanvas canvas) == false)
            {
                return null;
            }

            if (closeOtherCanvas)
            {
                CloseAllCanvas();
            }

            canvas.OnOpened();

            return (T)canvas;
        }

        public T CloseCanvas<T>() where T : ViewCanvas
        {
            if (canvases.TryGetValue(typeof(T), out ViewCanvas canvas) == false)
            {
                return null;
            }

            canvas.OnClosed();

            return (T)canvas;
        }

        public void CloseAllCanvasExcept<T>() where T : ViewCanvas
        {
            foreach (var pair in canvases)
            {
                if (pair.Key == typeof(T))
                {
                    continue;
                }

                pair.Value.OnClosed();
            }
        }
       
        
        public void CloseAllCanvas()
        {
            foreach (var pair in canvases)
            {
                pair.Value.OnClosed();
            }
        }

        private void GetAllCanvas()
        {
            foreach (var canvas in canvasList)
            {
                canvases.Add(canvas.GetType(), canvas);
            }
        }
    }
}
