using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
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