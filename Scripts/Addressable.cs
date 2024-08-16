public class Addressable
{
    // Фрагмент кода обертки над Addressable 
    private readonly Dictionary<string, AsyncOperationHandle> _handles = new();
    
       public async Task Load()
       {
           var operation = Addressables.InitializeAsync();
           await operation.Task;
       }

       public async Task<TResource> GetAsset<TResource>(string key) where TResource : class
       {
           var exist = CheckUploadedResource(key);
           if (exist)
               await ReleaseAsset(key);

           var handleResource = Addressables.LoadAssetAsync<TResource>(key);

           Debug.Log($"Send Asset (key: {key}, Type: {typeof(TResource)})");
           return await AssetCreate(key, handleResource);
       }
       
        private async Task<TResource> AssetCreate<TResource>(string key, AsyncOperationHandle<TResource> handle)
        {
            var loadedAsset = await handle.Task;
            _handles.Add(key, handle);
            
            return loadedAsset;
        }

        public Task ReleaseAsset(string key)
        {
            if (!CheckPresenceOfDictionary(key, out var handle))
            {
                Debug.Log($"No such AssetGUID. Perhaps the asset has not been created yet  (key: {key})");
                return Task.CompletedTask;
            }
            
            _handles.Remove(key);
            Addressables.Release(handle);
            Resources.UnloadUnusedAssets();
            
            Debug.Log($"Release Asset (key: {key})");
            
            return Task.CompletedTask;
        }

        public Task ReleaseAllAsset()
        {
            foreach (var handle in _handles)
                Addressables.Release(handle.Value);
            
            Resources.UnloadUnusedAssets();
            _handles.Clear();

            return Task.CompletedTask;
        }

        private bool CheckPresenceOfDictionary(string key, out AsyncOperationHandle handle)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                handle = default;
                return false;
            }

            if (_handles.ContainsKey(key!))
                return _handles.TryGetValue(key, out handle);

            handle = default;
            return false;
        }

        private bool CheckUploadedResource(string key)
        {
            return _handles.TryGetValue(key, out var handle);
        }
}