using AKSIngress.Business;
using Microsoft.AspNetCore.Mvc;

namespace AKSIngress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngressController : ControllerBase
    {
        private readonly IBusinessDetails _businessDetails;
        public IngressController(IBusinessDetails businessDetails)
        {
            _businessDetails = businessDetails;
        }
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_businessDetails.GetDetails());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}
