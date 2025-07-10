using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public interface ISetupWindow
    {
        string GetWindowTitle();
        ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window);
        /// <summary>
        /// Draw the window buttons
        /// </summary>
        /// <param name="width">with of the window</param>
        /// <param name="height">height of the window</param>
        /// <returns>false if closed</returns>
        bool DrawInWindow(float width, float height);
        void DrawInScene();
        void MouseMove(Vector3 mousePosition);
        void LeftClick(Vector3 mousePosition, bool clicked);
        //void LeftClick(Vector3 mousePosition);
        void RightClick(Vector3 mousePosition);
        void UndoAction();
        void DestroyWindow();
        string GetFullClassName();
        bool GetBlockClicksState();
        void InspectorUpdate();
    }
}