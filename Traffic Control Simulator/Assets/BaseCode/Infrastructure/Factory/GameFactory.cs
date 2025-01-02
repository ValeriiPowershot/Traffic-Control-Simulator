using BaseCode.Infrastructure.AssetManagment;
using Infrastructure.AssetManagment;
using UnityEngine;

namespace BaseCode.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assets;

        public GameFactory(IAssetsProvider assets) =>
            _assets = assets;

        public GameObject CreateHUD() =>
            _assets.Instantiate(AssetPath.HUDPath);
    }
}
