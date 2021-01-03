namespace wcs.Lang
{
    public static class MLang
    {
        public static string GetLang(string key) => Lang.ResourceManager.GetString(key);

    }
}
