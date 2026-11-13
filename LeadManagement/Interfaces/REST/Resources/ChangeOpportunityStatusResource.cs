using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record ChangeOpportunityStatusResource([Required(ErrorMessage = "RequiredField"), StringLength(10, ErrorMessage = "LengthLimit")] string Status);
