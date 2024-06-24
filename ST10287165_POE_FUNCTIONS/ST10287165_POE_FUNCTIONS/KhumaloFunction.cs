using CLDV6211_ST10287165_POE_P1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ST10287165_POE_FUNCTIONS
{
    public static class KhumaloFunction
    {
        [FunctionName("ProcessOrderConfirmation")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var confirmationDetails = JsonConvert.DeserializeObject<dynamic>(requestBody);

            if (confirmationDetails == null)
            {
                return new NotFoundResult(); // Handle cases where the data is not correctly deserialized
            }

            // Log confirmation or send a notification email
            log.LogInformation($"Order {confirmationDetails.OrderId} has been confirmed.");

            return new OkObjectResult($"Confirmation received for Order ID: {confirmationDetails.OrderId}");
        }
    }
}