using Common;
using Data.Entities;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace BookManagementAPI.ErrorHelper
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GlobalExceptionHandler));
        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is IntendForUserException)
            {
                //context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "some bug"));
            }
            else if(context.Exception is ApiDataException)
            {
                var dataException = context.Exception as ApiDataException;
                //string message = string.Format("ReasonPhares: {0} - ErrorDescription: {1}", dataException.ReasonPhrase, dataException.ErrorDescription);
                //context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(dataException.HttpStatus, message));
                context.Result = new ResponseMessageResult(context.Request.CreateResponse(dataException.HttpStatus, new ServiceStatus() { ReasonPhrase = dataException.ReasonPhrase, StatusMessage =dataException.ErrorDescription}));
                _logger.Error(string.Format("ReasonPhares: {0} - ErrorDescription: {1}", dataException.ReasonPhrase, dataException.ErrorDescription));
            }
            else if (context.Exception is ApiException)
            {

            }
            else context.Result = new ResponseMessageResult(context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Undefined Error"));
        }
    }
}