using BaseCode.Services;
using Services;
using UnityEngine;

namespace Infrastructure.AssetManagment
{
    public interface IAssetsProvider : IService
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector3 at);
    }
}
