using System;

namespace AKSIngress.Business
{
    public class BusinessDetails : IBusinessDetails
    {
        public string GetDetails()
        {
            return $"This request is being served by : {Environment.MachineName} having version {Environment.GetEnvironmentVariable("APP_NAME")} ";
        }
    }
}
