using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record ContactTagResource(long ContactId, long TagId);
public record AssignTagResource([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long TagId);
