namespace webtod.Contracts;

public record NoteDto(Guid id, string title, string description, DateTime createdAt, DateTime updatedAt);
