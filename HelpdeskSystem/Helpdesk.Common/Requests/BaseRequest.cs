using Helpdesk.Common.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace Helpdesk.Common.Requests
{
    /// <summary>
    /// Used as the base for all requests so that they all are validatable
    /// </summary>
    public class BaseRequest : IValidatableObject
    {
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }

        public BaseResponse CheckValidation(BaseResponse response)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this);
            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            if (!isValid)
            {
                response.Status = HttpStatusCode.BadRequest;

                foreach (ValidationResult result in validationResults)
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, result.ErrorMessage));
            }

            return response;
        }
    }
}
