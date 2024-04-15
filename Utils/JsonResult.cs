using System.Net.Mime;
using System.Text;
static class ResultsExtensions
{
    public static IResult Json(this IResultExtensions resultExtensions, string msg, object data)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new JsonResult(0, msg, data);
    }
}

class JsonResult(int code, string msg, object data) : IResult
{
    record FramedResult(int Code, string Msg, object Data);
    readonly FramedResult data = new(code, msg, data);
    public Task ExecuteAsync(HttpContext httpContext) =>
        httpContext.Response.WriteAsJsonAsync(data);
    
    public static JsonResult Ok(object data) => new(0, "Ok", data);
}