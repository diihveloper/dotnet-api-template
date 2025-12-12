
namespace DiihTemplate.Core.Dtos;
/// <summary>
/// Classe auxiliar para encapsular informações de erro
/// </summary>
public class ErrorInfo
{
    public string? Error { get; set; }
    public string? StackTrace { get; set; }
    public string? ErrorType { get; set; }
    public string Response { get; set; } = string.Empty;
}