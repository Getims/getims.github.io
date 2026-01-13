using UnityEngine;

namespace Project.Scripts.Infrastructure.Assets
{
    public interface IAssetProvider
    {
        T Load<T>(string path) where T : Object;
    }
}