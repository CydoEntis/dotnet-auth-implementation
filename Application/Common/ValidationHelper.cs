using AuthImplementation.Infrastructure.Common;
using FluentValidation;

namespace AuthImplementation.Application.Common;

public static class ValidationHelper
{
    public static async Task<Result<TResponse>?> ValidateAsync<TRequest, TResponse>(
        IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => new ValidationError
            {
                Property = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            var apiError = new ApiError
            {
                Code = ErrorCode.ValidationError,
                Message = "One or more validation errors occurred.",
                Details = validationErrors
            };

            return Result<TResponse>.Failure(apiError);
        }

        return null;
    }
}