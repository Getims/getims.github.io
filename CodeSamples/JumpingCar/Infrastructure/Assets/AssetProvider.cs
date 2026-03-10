using UnityEngine;

namespace Project.Scripts.Infrastructure.Assets
{
    public class AssetProvider : IAssetProvider
    {
        public T Load<T>(string path) where T : Object =>
            Resources.Load<T>(path);
    }
}