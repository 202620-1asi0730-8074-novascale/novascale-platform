using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record SignInResource([Required(ErrorMessage = "RequiredField"), EmailAddress(ErrorMessage = "InvalidEmail"), StringLength(120, ErrorMessage = "LengthLimit")] string Email, [Required(ErrorMessage = "RequiredField"), StringLength(128, ErrorMessage = "LengthLimit")] string Password);
public record SignUpResource([Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string FullName, [Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string Company,
    [Required(ErrorMessage = "RequiredField"), EmailAddress(ErrorMessage = "InvalidEmail"), StringLength(120, ErrorMessage = "LengthLimit")] string Email, [Required(ErrorMessage = "RequiredField"), StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidPassword")] string Password);
public record UpdateProfileResource([Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string FullName, [StringLength(30, ErrorMessage = "LengthLimit")] string Phone, [property: System.Text.Json.Serialization.JsonRequired] decimal Quota, [StringLength(120, ErrorMessage = "LengthLimit")] string? Company = null);
public record ChangePasswordResource([Required(ErrorMessage = "RequiredField"), StringLength(128, ErrorMessage = "LengthLimit")] string CurrentPassword, [Required(ErrorMessage = "RequiredField"), StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidPassword")] string NewPassword);
public record AuthenticationResource(string Token, UserResource User);
