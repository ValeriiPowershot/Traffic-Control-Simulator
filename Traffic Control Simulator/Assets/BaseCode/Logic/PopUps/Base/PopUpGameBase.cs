namespace BaseCode.Logic.PopUps.Base
{
    public class PopUpGameBase : PopUpBase
    {
        private GameManager _gameManager;

        protected GameManager GameManager
        {
            get
            {
                if (_gameManager == null)
                    _gameManager = FindObjectOfType<GameManager>();

                return _gameManager;
            }
        }
        
        public PopUpManager PopUpManager => GameManager.popUpManager;
        
    }
}