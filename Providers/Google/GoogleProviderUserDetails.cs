namespace Invitee.Providers.Google
{
    public class GoogleProviderUserDetails
    {
        public string Email { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string Locale { get; internal set; }
        public string Name { get; internal set; }
        public string ProviderUserId { get; internal set; }
        public string ProfilePic { get; internal set; }
    }
}