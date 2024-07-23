namespace webtod.Contracts;

public record UpdateNoteRequest(Guid id, string title, string description);