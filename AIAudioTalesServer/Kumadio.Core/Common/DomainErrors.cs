namespace Kumadio.Core.Common
{
    public static class DomainErrors
    {
        public static class Transaction
        {
            public static Error Failed(string message) => new(ErrorCodes.TransactionFailed, message);
        }
        public static class Auth
        {
            public static Error UserEmailNotFound => new (ErrorCodes.UserEmailNotFound, "User with that email does not exist.");
            public static Error EmailAlreadyExists => new (ErrorCodes.EmailAlreadyExists, "User with that email already exist.");
            public static Error WrongPassword => new (ErrorCodes.WrongPassword, "Password is not correct.");
            public static Error RefreshTokenNotFound => new (ErrorCodes.RefreshTokenNotFound, "Refresh token was not found.");
            public static Error UserWithTokenNotFound => new (ErrorCodes.UserWithTokenNotFound, "User with that refresh token was not found.");
            public static Error RefreshTokenExpired => new (ErrorCodes.RefreshTokenExpired, "Refresh token date has expired.");
            public static Error GoogleCredentialsNotValid => new (ErrorCodes.GoogleCredentialsNotValid, "Google credentials for this account are not valid.");
            public static Error JwtTokenMissing => new (ErrorCodes.JwtTokenMissing, "Jwt token is missing from request cookie.");
            public static Error RefreshTokenMissing => new (ErrorCodes.RefreshTokenMissing, "Refresh token is missing from request cookie.");
            public static Error EmailClaimMissing => new (ErrorCodes.EmailClaimMissing, "Email claim is missing from jwt token.");
            public static Error UserNull => new (ErrorCodes.UserNull, "User object cannot be null.");
        }

        public static class Catalog
        {
            public static Error BooksNotFound => new(ErrorCodes.BooksNotFound, "Books are not found.");
            public static Error CategoriesNotFound => new(ErrorCodes.CategoriesNotFound, "Categories are not found.");
            public static Error BookNotFound => new(ErrorCodes.BookNotFound, "Book with that id is not found.");
            public static Error BookPartNotFound => new(ErrorCodes.BookPartNotFound, "Book part with that id is not found.");
            public static Error RootPartNotFound => new(ErrorCodes.RootPartNotFound, "Root part for that book id is not found.");

        }
    }
}
