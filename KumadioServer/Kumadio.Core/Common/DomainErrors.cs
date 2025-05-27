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
            public static Error UserNull => new (ErrorCodes.UserNull, "User object cannot be null.");
            public static Error JwtTokenIssue => new (ErrorCodes.JwtTokenIssue, "Issue with creating new jwt token.");
            public static Error UserAlreadyOnboarded => new (ErrorCodes.UserAlreadyOnboarded, "User has already onboarded.");
        }

        public static class Catalog
        {
            public static Error CategoriesNotFound => new(ErrorCodes.CategoriesNotFound, "Categories are not found.");
            public static Error BookNotFound => new(ErrorCodes.BookNotFound, "Book with that id is not found.");
            public static Error BookPartNotFound => new(ErrorCodes.BookPartNotFound, "Book part with that id is not found.");
        }

        public static class Editor
        {
            public static Error BookNotFound => new(ErrorCodes.BookNotFound, "Book with that id is not found.");
            public static Error SaveFileFailed => new(ErrorCodes.SaveFileFailed, "Save of audio file failed.");
            public static Error InvalidParentAnswer => new(ErrorCodes.InvalidParentAnswer, "Invalid parent answer.");

        }

        public static class Library
        {
            public static Error PurchasedBookNotFound => new(ErrorCodes.PurchasedBookNotFound, "Purchased book for that user is not found.");
            public static Error CurrentBookNotFound => new(ErrorCodes.CurrentBookNotFound, "Current book for that user is not found.");
            public static Error InvalidBook => new(ErrorCodes.InvalidBook, "Book with that id is not valid for adding to library.");
            public static Error NextPartNotFound => new(ErrorCodes.NextPartNotFound, "Next part with that id is not found.");
            public static Error NextBookNotFound => new(ErrorCodes.NextBookNotFound, "Next book with that id is not found.");
            public static Error InvalidSearchTerm => new(ErrorCodes.InvalidSerachTerm, "Search term value is invalid.");
        }

        public static class Common
        {
            public static Error RootPartNotFound => new(ErrorCodes.RootPartNotFound, "Root part for that book id is not found.");
        }
    }
}
