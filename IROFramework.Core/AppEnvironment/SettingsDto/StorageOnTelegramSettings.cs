namespace IROFramework.Core.AppEnvironment.SettingsDto
{
    public class StorageOnTelegramSettings
    {
        public string BotToken { get; set; }

        public long SaveResourcesChatId { get; set; }

        public bool DeletePreviousMessages { get; set; }

        public bool SaveOnSet { get; set; }

        public bool LoadOnGet { get; set; }
        
        public int AutoSavesDelaySeconds { get; set; }

        public bool CacheFilesAndNotWait { get; set; }

        public bool DeleteOlderFiles { get; set; }
    }
}