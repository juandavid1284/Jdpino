using jdpino.Common.Models;
using jdpino.Common.Responses;
using Jdpino.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Jdpino.Functions.Funtions
{
    public static class TodoApi
    {


        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,

            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        ILogger log)
        {
            log.LogInformation("Recieved a new todo.");

            string RequestBody = null;


           Todo todo = JsonConvert.DeserializeObject<Todo>(RequestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucces = false,
                    Message = "The Request must have a TaskDescription"
                });

            }

            TodoEntity todoEntity = new TodoEntity

            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription

            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = "New todo estored in table";
            log.LogInformation(message);





            return new OkObjectResult(new Response
            {

                IsSucces = true,
                Message = message,
                Result = todoEntity

            });

        }
    }
}



