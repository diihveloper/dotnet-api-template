using System.Net;
using System.Text.Json;

namespace DiihTemplate.Core.Dtos;

public class ResultDto
{
    public bool SuccessResult { get; set; }
    public virtual HttpStatusCode StatusCode { get; }
    public string? Message { get; }

    public static implicit operator string(ResultDto v) => v.ToString();

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public ResultDto(bool success, HttpStatusCode status, string? message)
    {
        SuccessResult = success;
        StatusCode = status;
        Message = message;
    }

    public static ResultDto Success<T>(T? data, string message, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ResultDto<T>(true, status, data, message);
    }

    public static ResultDto Success(string? message = null, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new ResultDto(true, status, message);
    }

    public static ResultDto Error<T>(string message, HttpStatusCode status = HttpStatusCode.InternalServerError,
        T? data = default)
    {
        return new ResultDto<T>(false, status, data, message);
    }

    public static ResultDto Error(string message, HttpStatusCode status = HttpStatusCode.InternalServerError)
    {
        return new ResultDto(false, status, message);
    }

    public static ResultDto BadRequest<T>(string message, T? data = default)
    {
        return new ResultDto<T>(false, HttpStatusCode.BadRequest, data, message);
    }

    public static ResultDto BadRequest(string message)
    {
        return new ResultDto(false, HttpStatusCode.BadRequest, message);
    }
}

public class ResultDto<T> : ResultDto
{
    public T? Data { get; set; }

    public ResultDto(bool success, HttpStatusCode status, T? data, string? message) : base(success, status, message)
    {
        Data = data;
    }
}