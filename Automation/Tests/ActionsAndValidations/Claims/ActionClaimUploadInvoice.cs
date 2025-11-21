using Rac.TestAutomation.Common;
using System.Collections.Generic;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Claim.UploadInvoice;


namespace Tests.ActionsAndValidations.Claims
{
    public static class ActionClaimUploadInvoice
    {
        /// <summary>
        /// Upload and Submit Invoice
        /// Click Submit
        /// </summary>
       
        public static void UploadAndSubmitInvoice(Browser browser, ClaimUploadFile claim)
        {
            using (var uploadAndSubmit = new UploadAndSubmit(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: uploadAndSubmit);
                uploadAndSubmit.UploadInvoiceAndSubmit(claim);
            }
        }

        /// <summary>
        /// Upload all files
        /// </summary>
        public static void UploadFiles(Browser browser, ClaimUploadFile claim)
        {
            using (var uploadAndSubmit = new UploadAndSubmit(browser))           
            {
                uploadAndSubmit.UploadFileAndCheckStatus(claim);
            }
        }

        /// <summary>
        /// Delete files
        /// </summary>
        public static void DeleteFiles(Browser browser, List<string> alreadyUploadedFiles, List<string> filesToBeDeleted)
        {
            using (var uploadAndSubmit = new UploadAndSubmit(browser))
            {
                uploadAndSubmit.DeleteFiles(alreadyUploadedFiles, filesToBeDeleted);
            }
        }

        /// <summary>
        /// Submit invoice upload
        /// </summary>
        public static void SubmitFiles(Browser browser)
        {
            using (var uploadAndSubmit = new UploadAndSubmit(browser))
            {
                uploadAndSubmit.ClickSubmit();
            }
        }

        /// <summary>
        /// Upload a file which is not supported
        /// verify the error message
        /// </summary>
        public static void UploadAnVerifyError(Browser browser, ClaimUploadFile claim)
        {
            using (var uploadAndSubmit = new UploadAndSubmit(browser))
            {
                uploadAndSubmit.UploadFileError(claim);
            }
        }

        /// <summary>
        /// Verify details on Confirmation page        
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void VerifyConfirmationPage(Browser browser, ClaimUploadFile claim)
        {
            using (var confirmationPage = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: confirmationPage);
                

                confirmationPage.VerifyConfirmationPage(claim);
            }
        }

        /// <summary>
        /// Verify File limits reached error page        
        /// </summary>      
        public static void VerifyMaximumFileLimitReachedErrorPage(Browser browser, string claimNumber)
        {
            using (var errorPage = new ErrorPage(browser))           
            {
                errorPage.VerifyFileLimitReachedErrorPage(claimNumber);
            }
        }

        /// <summary>
        /// Verify Link expired error page        
        /// </summary>      
        public static void VerifyLinkExpiredErrorPage(Browser browser, string claimNumber)
        {
            using (var errorPage = new ErrorPage(browser))
            {
                errorPage.VerifyLinkExpiredErrorPage(claimNumber);
            }
        }

        /// <summary>
        /// Verify Something went wrong error page        
        /// </summary>      
        public static void VerifySomethingWentWrongErrorPage(Browser browser, string claimNumber)
        {
            using (var errorPage = new ErrorPage(browser))
            {
                errorPage.VerifySomethingWentWrongErrorPage(claimNumber);
            }
        }

        /// <summary>
        /// Verify session time out error page        
        /// </summary>      
        public static void VerifySessionTimeOutErrorPage(Browser browser)
        {
            using (var errorPage = new ErrorPage(browser))
            {
                errorPage.VerifySessionTimeOutErrorPage();
            }
        }
    }
}
