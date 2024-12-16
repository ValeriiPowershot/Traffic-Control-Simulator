using BaseCode.Services;
using UnityEngine;

namespace BaseCode.Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        GameObject CreateHUD();
    }
}
