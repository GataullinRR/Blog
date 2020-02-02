using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace PerformanceTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class CommandHandlerAttribute : Attribute
    {
        public CommandHandlerAttribute(string commandName) 
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        }
        public CommandHandlerAttribute(string commandName, string commandDescription)
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
            CommandDescription = commandDescription ?? throw new ArgumentNullException(nameof(commandDescription));
        }

        public string CommandName { get; }
        public string CommandDescription { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string parameterName, string parameterDescription)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterDescription = parameterDescription;
        }
        public ParameterAttribute(string parameterName) :this(parameterName, null)
        {

        }
        public string ParameterName { get; }
        public string ParameterDescription { get; }
    }

    class Program
    {
        class RequestInfo 
        {
            readonly Stopwatch _sw = Stopwatch.StartNew();
            readonly Task<HttpResponseMessage> _responseFuture;
            readonly Task _future;

            public HttpResponseMessage Response => _responseFuture.Result;
            public string Content { get; private set; }
            public double Duration => _sw.Elapsed.TotalMilliseconds;
            public bool IsCompleted => _future.IsCompleted;
         
            public RequestInfo(Task<HttpResponseMessage> responseFuture)
            {
                _responseFuture = responseFuture ?? throw new ArgumentNullException(nameof(responseFuture));
                _future = _responseFuture.ContinueWith(async r => Content = await r.Result.Content.ReadAsStringAsync());
                _future.GetAwaiter().OnCompleted(() => _sw.Stop());
            }

            public async Task WaitCompletionAsync()
            {
                await _future;
            }

            public override string ToString()
            {
                return $"The task has been completed in {Duration} ms with status {Response.StatusCode} with content length {Content.Length} with target {Response.RequestMessage.RequestUri}";
            }
        }

        static async Task Main(string[] args)
        {
            var commands = (from mi in typeof(Program).GetMethods(BindingFlags.Static | BindingFlags.Public)
                           let info = mi.GetCustomAttribute<CommandHandlerAttribute>()
                           where info != null
                           let parameters = (from pi in mi.GetParameters()
                                            let pInfo = pi.GetCustomAttribute<ParameterAttribute>() ?? new ParameterAttribute(pi.Name, pi.Name)
                                            select new { Type = pi.ParameterType, Info = pInfo }).ToArray()
                           select new { Handler = mi, HandlerInfo = info, Parameters = parameters  }).ToArray();

            Console.WriteLine("Following commands are awailable:");
            foreach (var command in commands)
            {
                Console.WriteLine($"{command.HandlerInfo.CommandName} - {command.HandlerInfo.CommandDescription ?? command.Handler.Name}");
                if (command.Parameters.Any())
                {
                    Console.WriteLine("   With parameters:");
                    foreach (var parameter in command.Parameters)
                    {
                        Console.WriteLine($"   [{parameter.Type}] -{parameter.Info.ParameterName} {parameter.Info.ParameterDescription}");
                    }
                }
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter command:");

                var command = Console.ReadLine();
                var parts = command.Split(" ");
                var commandName = parts[0];
                var info = commands.FirstOrDefault(c => c.HandlerInfo.CommandName == commandName);
                if (info == null)
                {
                    Console.WriteLine("Command is not found");
                }
                else
                {
                    var parameters = parts.Skip(1)
                        .GroupBy(2)
                        .Select(g => g.ToArray())
                        .ToArray();
                    if (parameters.Length == info.Parameters.Length)
                    {
                        var parsedParameters = new object[info.Parameters.Length];
                        for (int i = 0; i < info.Parameters.Length; i++)
                        {
                            if (parameters[i][0].Skip(1).Aggregate() == info.Parameters[i].Info.ParameterName)
                            {
                                if (info.Parameters[i].Type == typeof(int))
                                {
                                    parsedParameters[i] = parameters[i][1].ParseToInt32Invariant();
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }


                        await (Task)info.Handler.Invoke(null, parsedParameters);
                    }
                    else
                    {
                        Console.WriteLine("Wrong number of parameters");
                    }
                }
            }
        }

        [CommandHandler("pt", "Post requests")]
        public static async Task PostReq([Parameter("nor")]int numOfRequests, [Parameter("ic")]int connectionsCount, [Parameter("rd")]int delay)
        {
            using (var client = new HttpClient())
            {
                await waitServerLoadsAsync(client);

                var response = await client.GetAsync("http://localhost:14971/Testing/GetAllPostsAsync");
                var content = await response.Content.ReadAsStringAsync();
                var uris = JsonSerializer.Deserialize<List<string>>(content);

                var futures = new List<RequestInfo>();
                var urisIterator = uriStream().Take(numOfRequests).ToArray().StartEnumeration();
                while (futures.Count < numOfRequests)
                {
                    if (futures.Count(f => !f.IsCompleted) < connectionsCount )
                    {
                        await Task.Delay(delay);
                        var uri = urisIterator.AdvanceOrThrow();
                        futures.Add(new RequestInfo(client.GetAsync(uri)));
                    }
                    else
                    {
                        await Task.Delay(10);
                    }
                }
                foreach (var future in futures)
                {
                    await future.WaitCompletionAsync();
                    Console.WriteLine(future);
                }

                showStatistic(futures);

                Console.WriteLine("Benchmark has been completed");

                IEnumerable<string> uriStream()
                {
                    for (int i = 0; i < numOfRequests; i++)
                    {
                        yield return Global.Random.NextElementFrom(uris.ToArray());
                    }
                }
            }

            async Task waitServerLoadsAsync(HttpClient client)
            {
                Console.WriteLine("Waiting for server to complete loading...");
                HttpResponseMessage response = null;
                do
                {
                    try
                    {
                        response = await client.GetAsync("http://localhost:14971/");
                    }
                    catch (HttpRequestException ex)
                    {
                        await Task.Delay(1000);
                    }
                }
                while (response == null || response.StatusCode != HttpStatusCode.OK);

                await Task.Delay(3000); // Give it some additional time
                Console.WriteLine("Server has been loaded!");
            }

            void showStatistic(IList<RequestInfo> requests)
            {
                var avgResponseTime = requests.Sum(r => r.Duration) / requests.Count;
                var minResponseTime = requests.Min(r => r.Duration);
                var maxResponseTime = requests.Max(r => r.Duration);

                Console.WriteLine($"Min response time: {minResponseTime}");
                Console.WriteLine($"Max response time: {maxResponseTime}");
                Console.WriteLine($"Avg response time: {avgResponseTime}");
            }
        }
    }
}
