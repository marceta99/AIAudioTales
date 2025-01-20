namespace Kumadio.Core.Common
{
    public static class ErrorCodes
    {
        public const string TransactionFailed = "TRANSACTION_FAILED";
        public const string UserEmailNotFound = "USER_EMAIL_NOT_FOUND";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
        public const string WrongPassword = "WRONG_PASSWORD";
        public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
        public const string UserWithTokenNotFound = "USER_WITH_TOKEN_NOT_FOUND";
        public const string JwtTokenMissing = "JWT_TOKEN_MISSING";
        public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";
        public const string GoogleCredentialsNotValid = "GOOGLE_CREDENTIALS_NOT_VALID";
        public const string EmailClaimMissing = "EMAIL_CLAIM_MISSING";
    }
}
