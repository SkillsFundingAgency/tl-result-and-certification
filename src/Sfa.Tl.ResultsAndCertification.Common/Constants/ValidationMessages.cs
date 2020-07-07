﻿namespace Sfa.Tl.ResultsAndCertification.Common.Constants
{
    public class ValidationMessages
    {
        // Todo: can this move to resource file? assess. 
        // Property validation messages
        public const string Required = "{0} required";
        public const string MustBeNumberWithLength = "{0} must be a {1} digit number";
        public const string MustBeAnNumberWithLength = "{0} must be an {1} digit number";
        public const string StringLength = "{0} cannot have more than {1} characters";
        public const string MustBeStringWithLength = "{0} must have {1} characters only";
        public const string MustBeValidDate = "{0} must be a valid date in DDMMYYYY format";
        public const string DateNotinFuture = "{0} must be in the past";

        // File based validation messages
        public const string FileHeaderNotFound = "File header is not valid";
        public const string NoRecordsFound = "No registration data received";
        public const string DuplicateRecord = "Duplicate ULN found";
        public const string InvalidColumnFound = "Data in more than the required {0} columns";

        // Generic or unexpected behaviour messages
        public const string UnableToParse = "Unable to parse the row.";
        public const string UnableToReadCsvData = "Unable to interpret content.";
        public const string UnexpectedError = "Unexpected error while reading file content.";

        // Bulk Registration Stage3 Validation Messages
        public const string ProviderNotRegisteredWithAo = "Provider not registered with awarding organisation";
        public const string CoreNotRegisteredWithProvider = "Core not registered with provider";
        public const string SpecialismNotValidWithCore = "Specialism not valid with core";

        // Bulk Registration Stage4 Validation Messages
        public const string ActiveUlnWithDifferentAo = "Active ULN with a different awarding organisation";
        public const string CoreForUlnCannotBeChangedYet = "Core for ULN cannot be changed yet";
    }
}
