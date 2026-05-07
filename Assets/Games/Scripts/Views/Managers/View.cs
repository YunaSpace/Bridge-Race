namespace YunaSpace.BridgeRace
{
    public static class View
    {
        public static ViewManager Manager => ViewManager.Instance;

        public static void OpenCanvas<T>(bool closeOtherCanvas = false) where T : ViewCanvas => Manager.OpenCanvas<T>(closeOtherCanvas);
        public static void CloseCanvas<T>() where T : ViewCanvas => Manager.CloseCanvas<T>();
        public static void CloseAllCanvas() => Manager.CloseAllCanvas();
        public static void CloseAllCanvasExcept<T>() where T : ViewCanvas => Manager.CloseAllCanvasExcept<T>();
    }
}