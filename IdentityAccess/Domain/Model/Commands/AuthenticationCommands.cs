namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

public record SignInCommand(string Email, string Password);
public record SignUpCommand(string FullName, string Company, string Email, string Password);
public record UpdateProfileCommand(string FullName, string Phone, decimal Quota, string? Company = null);
public record ChangePasswordCommand(string CurrentPassword, string NewPassword);
