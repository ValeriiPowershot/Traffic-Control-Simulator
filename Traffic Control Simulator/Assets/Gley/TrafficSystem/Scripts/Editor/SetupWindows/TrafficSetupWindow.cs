using Gley.UrbanAssets.Editor;
using UnityEditor;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficSetupWindow : SetupWindowBase
    {
        protected TrafficSettingsWindowData editorSave;
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            editorSave = new SettingsLoader(Internal.Constants.windowSettingsPath).LoadSettingsAsset<TrafficSettingsWindowData>();
            return this;
        }

        public override void DestroyWindow()
        {
            EditorUtility.SetDirty(editorSave);
            base.DestroyWindow();
        }
    }
}
