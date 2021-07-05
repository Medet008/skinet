using System;

namespace API.Errors
{
  public class ApiResponse
  {
    public ApiResponse(int statusCode, string message = null)
    {
      StatusCode = statusCode;
      // нулевой оператор объединения
      // Если null выполнить следующую строку
      Message = message ?? GetDefaultMessageForStatusCode(statusCode);
    }
    public int StatusCode { get; }
    public string Message { get; }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
      return statusCode switch {
          400 => "Вы сделали не правильный запрос",
          401 => "Вы не авторизован",
          403 => "Вам запрещено делать это,",
          404 => "Ресурс не было найден",
          500 => "Ошибки - это часть наивной стороне. Ошибки приводят к гневу. Гнев ведет к ненависти. Ненависть ведет к смене карьеры.",
          _ => null
      };
    }

  }
}