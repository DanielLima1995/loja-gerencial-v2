using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Loja.API.Controllers; // Lembre-se de ajustar para o namespace do seu projeto
    [Route("api/[controller]")]
    [ApiController]
//    [ApiExplorerSettings(IgnoreApi = true)] // Oculta este endpoint do Swagger
    public class DevController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("exportar-responses")]
        public IActionResult ExportarDocumentacaoCompleta()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sb = new StringBuilder();

            var controllers = assembly.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

            // Configuração para deixar o JSON bonito (indentado) e no padrão camelCase da API
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            foreach (var controller in controllers)
            {
                string controllerRoute = "";
                var routeAttr = controller.GetCustomAttribute<RouteAttribute>();
                if (routeAttr != null && routeAttr.Template != null)
                {
                    string nomeController = controller.Name.EndsWith("Controller")
                        ? controller.Name.Substring(0, controller.Name.Length - 10)
                        : controller.Name;

                    controllerRoute = routeAttr.Template.Replace("[controller]", nomeController);
                }

                sb.AppendLine($"\n=========================================");
                sb.AppendLine($"Controller: {controller.Name}");
                sb.AppendLine($"=========================================");

                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

                foreach (var method in methods)
                {
                    var httpMethodAttr = method.GetCustomAttributes().OfType<HttpMethodAttribute>().FirstOrDefault();
                    if (httpMethodAttr == null) continue;

                    string verboHttp = httpMethodAttr.HttpMethods.FirstOrDefault() ?? "UNKNOWN";
                    string methodRoute = httpMethodAttr.Template ?? "";

                    string rotaCompleta = $"/{controllerRoute}/{methodRoute}".Replace("//", "/").TrimEnd('/');
                    if (string.IsNullOrWhiteSpace(rotaCompleta)) rotaCompleta = "/";

                    sb.AppendLine($"\n[{verboHttp}] {rotaCompleta}");

                    // ==========================================
                    // 1. MAPEAMENTO DO REQUEST (Inputs, Headers, Body)
                    // ==========================================
                    sb.AppendLine($"  --- REQUEST ---");

                    var parameters = method.GetParameters()
                        .Where(p => p.GetCustomAttribute<FromServicesAttribute>() == null && p.ParameterType != typeof(CancellationToken))
                        .ToList();

                    var headerParams = new List<string>();
                    var queryParams = new List<string>();
                    Type bodyType = null;

                    // Verifica se o método é paginado baseado no retorno (Ex: PagedList)
                    Type returnType = ObterTipoReal(method.ReturnType);
                    bool isPaginado = returnType != null && returnType.Name.StartsWith("PagedList");

                    foreach (var p in parameters)
                    {
                        bool isHeader = p.GetCustomAttribute<FromHeaderAttribute>() != null;
                        bool isQuery = p.GetCustomAttribute<FromQueryAttribute>() != null;
                        bool isBody = p.GetCustomAttribute<FromBodyAttribute>() != null;
                        bool isRoute = p.GetCustomAttribute<FromRouteAttribute>() != null || rotaCompleta.Contains($"{{{p.Name}", StringComparison.OrdinalIgnoreCase);

                        if (isHeader)
                        {
                            headerParams.Add($"- {p.Name} ({p.ParameterType.Name})");
                        }
                        else if (isBody)
                        {
                            bodyType = p.ParameterType;
                        }
                        else if (isRoute)
                        {
                            queryParams.Add($"- [Route] {p.Name} ({p.ParameterType.Name})");
                        }
                        else
                        {
                            // Inferência padrão do ASP.NET
                            if (IsComplexType(p.ParameterType) && (verboHttp == "POST" || verboHttp == "PUT" || verboHttp == "PATCH"))
                                bodyType = p.ParameterType;
                            else
                                queryParams.Add($"- [Query] {p.Name} ({p.ParameterType.Name})");
                        }
                    }

                    // SE FOR PAGINADO: Injeta os headers automaticamente
                    if (isPaginado)
                    {
                        // Altere os nomes abaixo para os headers reais que sua API usa (ex: X-Pagination, Page, Limit, etc)
                        headerParams.Add("- X-Page-Number (Int32) [Paginação]");
                        headerParams.Add("- X-Page-Size (Int32) [Paginação]");
                    }

                    if (headerParams.Any())
                    {
                        sb.AppendLine($"  Headers esperados:");
                        foreach (var h in headerParams) sb.AppendLine($"    {h}");
                    }

                    if (queryParams.Any())
                    {
                        sb.AppendLine($"  Parâmetros (Rota/Filtros):");
                        foreach (var q in queryParams) sb.AppendLine($"    {q}");
                    }

                    if (bodyType != null)
                    {
                        sb.AppendLine($"  Body esperado:");
                        var bodyExemplo = GerarExemplo(bodyType);
                        string bodyJson = JsonSerializer.Serialize(bodyExemplo, jsonOptions);
                        sb.AppendLine(IdentarJson(bodyJson, 4));
                    }

                    if (!parameters.Any() && !isPaginado)
                    {
                        sb.AppendLine($"  (Nenhum parâmetro esperado)");
                    }

                    // ==========================================
                    // 2. MAPEAMENTO DO RESPONSE
                    // ==========================================
                    sb.AppendLine($"  --- RESPONSE ---");

                    var responseAttributes = method.GetCustomAttributes<ProducesResponseTypeAttribute>().ToList();

                    if (responseAttributes.Any())
                    {
                        foreach (var attr in responseAttributes)
                        {
                            if (attr.Type != null && attr.Type != typeof(void))
                            {
                                sb.AppendLine($"  -> Status {attr.StatusCode}:");
                                var objetoExemplo = GerarExemplo(attr.Type);
                                string json = JsonSerializer.Serialize(objetoExemplo, jsonOptions);
                                sb.AppendLine(IdentarJson(json, 4));
                            }
                            else
                            {
                                sb.AppendLine($"  -> Status {attr.StatusCode}: (Sem corpo / Void)");
                            }
                        }
                    }
                    else
                    {
                        if (returnType != null && returnType != typeof(void) && returnType != typeof(IActionResult))
                        {
                            sb.AppendLine($"  -> Status 200 (Inferido):");
                            var objetoExemplo = GerarExemplo(returnType);
                            string json = JsonSerializer.Serialize(objetoExemplo, jsonOptions);
                            sb.AppendLine(IdentarJson(json, 4));
                        }
                        else
                        {
                            sb.AppendLine($"  -> Retorno dinâmico (IActionResult sem tipagem).");
                        }
                    }
                }
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/plain", "Documentacao_Completa_API.txt");
        }

        // --- MÉTODOS AUXILIARES ---

        private bool IsComplexType(Type type)
        {
            if (type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid))
                return false;

            if (Nullable.GetUnderlyingType(type) != null)
                return IsComplexType(Nullable.GetUnderlyingType(type));

            return true;
        }

        private Type ObterTipoReal(Type type)
        {
            if (type == null) return null;

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Task<>) || type.GetGenericTypeDefinition() == typeof(ValueTask<>)))
                type = type.GetGenericArguments()[0];

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
                type = type.GetGenericArguments()[0];

            return type;
        }

        private string IdentarJson(string json, int espacos)
        {
            if (string.IsNullOrEmpty(json)) return "";
            var padding = new string(' ', espacos);
            return padding + json.Replace("\n", "\n" + padding);
        }

        private object GerarExemplo(Type type, int depth = 0)
        {
            if (depth > 3 || type == null) return null;

            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return 0;
            if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return 0.0;
            if (type == typeof(bool)) return false;
            if (type == typeof(DateTime)) return DateTime.UtcNow;
            if (type == typeof(Guid)) return Guid.NewGuid();

            if (Nullable.GetUnderlyingType(type) != null)
                return GerarExemplo(Nullable.GetUnderlyingType(type), depth);

            if (type.IsArray)
            {
                var itemType = type.GetElementType();
                var array = Array.CreateInstance(itemType, 1);
                array.SetValue(GerarExemplo(itemType, depth + 1), 0);
                return array;
            }

            if (type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                type.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                var itemType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(itemType);
                var list = (IList)Activator.CreateInstance(listType);
                list.Add(GerarExemplo(itemType, depth + 1));
                return list;
            }

            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);
                return values.Length > 0 ? values.GetValue(0) : 0;
            }

            if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
            {
                try
                {
                    object instance = null;
                    try
                    {
                        instance = Activator.CreateInstance(type);
                    }
                    catch
                    {
                        // Força a criação na memória caso a classe (como PagedList ou Records) não tenha construtor vazio
                        instance = System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);
                    }

                    if (instance != null)
                    {
                        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);
                        foreach (var prop in properties)
                        {
                            try { prop.SetValue(instance, GerarExemplo(prop.PropertyType, depth + 1)); }
                            catch { /* Ignora propriedades somente-leitura ou protegidas na injeção mockada */ }
                        }
                    }
                    return instance;
                }
                catch { return null; }
            }

            return null;
        }
    }
