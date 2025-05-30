﻿namespace Books.Api.Mapping
{
	using Contracts.Responses;
	using FluentValidation;

	public class ValidationMappingMiddleware(RequestDelegate next)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (ValidationException ex)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				var validationFailureResponse = new ValidationFailureResponse()
				{
					Errors = ex.Errors.Select(x => new ValidationResponse
					{
						PropertyName = x.PropertyName,
						Message = x.ErrorMessage
					})
				};

				await context.Response.WriteAsJsonAsync(validationFailureResponse);
			}
		}
	}
}