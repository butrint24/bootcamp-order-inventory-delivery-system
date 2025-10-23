namespace Shared.Entities
{
    public class UserAuth
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid UserId { get; private set; }
        public string PasswordHash { get; private set; } = null!;
        public string? RefreshToken  { get; private set; };
        public DateTime? RefreshTokenExpiryTime { get; private set; }


        public UserAuth(Guid userId, string passwordHash, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = refreshTokenExpiryTime;
        }
        public UserAuth(Guid userId, string passwordHash)
        {
            UserId = userId;
            PasswordHash = passwordHash;
        }

        public void UpdatePasswordHash(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void UpdateRefreshToken(string newRefreshToken, DateTime newExpiryTime)
        {
            RefreshToken = newRefreshToken;
            RefreshTokenExpiryTime = newExpiryTime;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

    }
}