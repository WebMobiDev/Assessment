namespace Assessment.Api.Errors;

public sealed class ConflictException : ApiException
{
    public ConflictException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status409Conflict;
}
