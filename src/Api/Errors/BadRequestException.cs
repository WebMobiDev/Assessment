namespace Assessment.Api.Errors;

public sealed class BadRequestException : ApiException
{
    public BadRequestException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status400BadRequest;
}
