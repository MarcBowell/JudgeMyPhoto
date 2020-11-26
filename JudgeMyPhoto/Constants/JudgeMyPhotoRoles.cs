namespace Marcware.JudgeMyPhoto.Constants
{
    public static class JudgeMyPhotoRoles
    {
        public const string Admin = "JudgeMyPhotoAdministrator";
        public const string Photographer = "JudgeMyPhotoPhotographer";
        public const string AllRoles = "JudgeMyPhotoAdministrator,JudgeMyPhotoPhotographer";
        public static string[] GetAllRoles()
        {
            return new string[]
            {
                Admin,
                Photographer
            };
        }
        public static string GetRoleDescription(string role)
        {
            switch (role)
            {
                case Admin:
                    return "Judge My Photo Administrator";
                case Photographer:
                    return "Photographer";
                default:
                    return string.Empty;
            }
        }
    }
}
