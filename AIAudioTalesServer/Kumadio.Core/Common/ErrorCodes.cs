namespace Kumadio.Core.Common
{
    public static class ErrorCodes
    {
        // Auth
        public const string TransactionFailed = "TRANSACTION_FAILED";
        public const string UserEmailNotFound = "USER_EMAIL_NOT_FOUND";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
        public const string WrongPassword = "WRONG_PASSWORD";
        public const string RefreshTokenNotFound = "REFRESH_TOKEN_NOT_FOUND";
        public const string UserWithTokenNotFound = "USER_WITH_TOKEN_NOT_FOUND";
        public const string JwtTokenMissing = "JWT_TOKEN_MISSING";
        public const string RefreshTokenMissing = "REFRESH_TOKEN_MISSING";
        public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";
        public const string GoogleCredentialsNotValid = "GOOGLE_CREDENTIALS_NOT_VALID";
        public const string EmailClaimMissing = "EMAIL_CLAIM_MISSING";
        public const string UserNull = "USER_NULL";

        // Catalog
        public const string CategoriesNotFound = "CATEGORIES_NOT_FOUND";
        public const string BookPartNotFound = "BOOK_PART_NOT_FOUND";
        public const string RootPartNotFound = "ROOT_PART_NOT_FOUND";

        // Editor
        public const string SaveFileFailed = "SAVE_FILE_FAILED";
        public const string InvalidParentAnswer = "InvalidParentAnswer";

        // Library
        public const string PurchasedBookNotFound = "PURCHASED_BOOK_NOT_FOUND";
        public const string CurrentBookNotFound = "CURRENT_BOOK_NOT_FOUND";

        // Common
        public const string BookNotFound = "BOOK_NOT_FOUND";

    }
}
