namespace BaseCode.Logic.PopUps.Base
{
    public class PopUpGameBase : PopUpBase
    {
        public GameManager gameManager;
        public PopUpController PopUpController => gameManager.popUpController;
    }
}