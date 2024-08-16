public class Localization
{
    // Фрагмент кода локализации проекта 
    // Wrapper это Monobehavior класс обертка над TextMeshPro, которая хранит в себе ключ локализации
    private readonly Dictionary<WrapperTMP, LocalizationWindow> _wrapperStorage = new();
    private Dictionary<string, TextSettings> _wordStorage = new();
    
    public void SwitchLanguage(Languages languages)
    {
        if (_lastLanguage == languages)
            return;

        _lastLanguage = languages;
        _localizationDataAccessor.SaveLanguage(languages);

        _wordStorage = _languageSwitcher.GetDictionary(languages);

        ChangeAllText();
    }

    private void ChangeAllText()
    {
        foreach (var wrapper in _wrapperStorage)
        {
            var key = wrapper.Key;
            key.TextSettings = SetValues(key.Id);
        }
    }
    
    public void AddWrapper(WrapperTMP wrapper, LocalizationWindow localizationWindow)
    {
        if (wrapper == null)
            throw new ArgumentNullException(nameof(wrapper));

        _wrapperStorage.TryAdd(wrapper, localizationWindow);

        ChangeText(wrapper);
    }

    public void ChangeText(WrapperTMP wrapper) => wrapper.TextSettings = SetValues(wrapper.Id);

    public void RemoveWrapper(WrapperTMP wrapper)
    {
        if (wrapper is null || !_wrapperStorage.ContainsKey(wrapper))
            return;

        _wrapperStorage.Remove(wrapper);
    }
    
    // Для удаления группы wrapper`ов например:
    // RemoveAllWrapper(localizationWindow.MainMenu)
    // RemoveAllWrapper(localizationWindow.InGame)
    public void RemoveAllWrapper(LocalizationWindow localizationWindow)
    {
        if (localizationWindow == LocalizationWindow.All)
        {
            _wrapperStorage.Clear();
            return;
        }

        // Находим все ключи, которые соответствуют данному значению
        var keysToRemove = _wrapperStorage.Where(pair => pair.Value == localizationWindow)
            .Select(pair => pair.Key)
            .ToList();

        // Удаляем найденные ключи
        foreach (var key in keysToRemove)
            _wrapperStorage.Remove(key);
    }
    
    private TextSettings SetValues(string id) => _wordStorage.GetValueOrDefault(id, _defaultSettings);
}