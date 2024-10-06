namespace FonTech.Domain.Enum;

public enum ErrorCodes
{
    ReportsNotFound = 0,
    ReportNotFound = 1,
    ReportAlreadyExists = 2,
    
    UserNotFound = 11,
    
    InternalServerError  = 10,
    
    PasswordNotEqualsPasswordConfirm = 21,
    UserAlreadyExits = 22,
    PasswordIsWrong = 23,
    UnauthorizedAccess = 24,
    
    RoleIsAsreadyExists = 31,
    RoleNotFound = 32,
    UserAlreadyHasThisRole = 33,
}