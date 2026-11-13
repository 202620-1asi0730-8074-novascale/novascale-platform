using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Repositories;

public interface IMessageRepository : IBaseRepository<Message> { }
