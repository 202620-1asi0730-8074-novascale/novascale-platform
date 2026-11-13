using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.OrganizationManagement.Interfaces.REST.Resources;

public record OrganizationResource(long Id, string Name, DateTime CreatedAt);
public record CreateOrganizationResource([Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string Name, [Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string OwnerName,
    [Required(ErrorMessage = "RequiredField"), EmailAddress(ErrorMessage = "InvalidEmail"), StringLength(120, ErrorMessage = "LengthLimit")] string OwnerEmail, [Required(ErrorMessage = "RequiredField"), StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidPassword")] string OwnerPassword);
public record UpdateOrganizationResource([Required(ErrorMessage = "RequiredField"), StringLength(120, ErrorMessage = "LengthLimit")] string Name);
public record ProvisionedOrganizationResource(OrganizationResource Organization, string OwnerToken);
