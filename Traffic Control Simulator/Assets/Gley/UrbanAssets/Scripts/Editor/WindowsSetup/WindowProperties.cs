namespace Gley.UrbanAssets.Editor
{
    public struct WindowProperties
    {
        public readonly string nameSpace;
        public readonly string className;
        public readonly string title;
        public readonly string tutorialLink;
        public readonly bool showBack;
        public readonly bool showTitle;
        public readonly bool showTop;
        public readonly bool showScroll;
        public readonly bool showBottom;
        public readonly bool blockClicks;


        public WindowProperties(string nameSpace, string className, string title, bool showBack, bool showTitle, bool showTop, bool showScroll, bool showBottom, bool blockClicks, string tutorialLink)
        {
            this.nameSpace = nameSpace;
            this.className = className;
            this.title = title;
            this.showBack = showBack;
            this.showTitle = showTitle;
            this.showTop = showTop;
            this.showScroll = showScroll;
            this.showBottom = showBottom;
            this.blockClicks = blockClicks;
            this.tutorialLink = tutorialLink;
        }
    }
}
