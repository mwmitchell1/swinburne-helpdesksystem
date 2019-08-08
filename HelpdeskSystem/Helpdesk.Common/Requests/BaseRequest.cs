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
        /// <summary>
        /// This will check the data annontations of the sub clas to ensure they are met and will
        /// return the desired error messages
        /// it can be overwritten to perform validation that can't be handled by the annotations
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns>List of validation errors</returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }

        /// <summary>
        /// Trigges the validation of the request
        /// </summary>
        /// <param name="response">The response that will contain the error results</param>
        /// <returns></returns>
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
