namespace Marcware.JudgeMyPhoto.Constants
{
    public static class CategoryStatusCodes
    {
        public const string SettingUp = "ST";
        public const string SubmittingPhotos = "OS";
        public const string Judging = "OJ";
        public const string Completed = "CM";

        public static string[] GetAll()
        {
            return new string[]
            {
                SettingUp,
                SubmittingPhotos,
                Judging,
                Completed
            };
        }

        public static string GetStatusText(string statusCode)
        {
            switch (statusCode)
            {
                case SettingUp:
                    return "Setting up";
                case SubmittingPhotos:
                    return "Open for photo submissions";
                case Judging:
                    return "Open for judging";
                case Completed:
                    return "Completed";
                default:
                    return "Unknown";
            }
        }
    }
}
