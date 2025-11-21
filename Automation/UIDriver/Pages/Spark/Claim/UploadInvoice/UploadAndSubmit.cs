using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using PercyIO.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.VisualTest;


namespace UIDriver.Pages.Spark.Claim.UploadInvoice
{


    public class UploadAndSubmit : SparkBasePage
    {

        private class Constants
        {
            public static string RemainingFileText(string number) => $"e.g. quotes, invoices and receipts. You can upload {number} for this claim.";
            public static readonly string MaxFileUploadText = "e.g. quotes, invoices and receipts. You've uploaded the maximum number of files.";
            public static readonly string NoFileUploadText = "Please upload a file";
            public static readonly string UnsupportedFileText = "Couldn't upload your file. File type not supported";
            public static readonly string ExceedsMaxFileSizeText = "Couldn't upload your file. It exceeds the maximum size of 8.5 MB";

        }

        private class XPath
        {
            public const string ClaimNumber = "id('claimNumberDisplay')";

            public class Field
            {
                public const string InputFile = "id('inputFile')";
                public const string RemainingFiles = "id('file-upload-input-sublabel')";
                public const string ErrorText = "id('file-upload-input-error-text')";
            }

            public class UploadedFile
            {
                public static string FileName(string file) => $"//div[contains(@class,'MuiGrid-root MuiGrid-container css') and .='{file}']";
            }

            public class Icon
            {
                public static string UploadFileStatus(string file) => $"//div[contains(@class,'MuiGrid-root MuiGrid-container css') and .='{file}']//*[(local-name()='svg') and @data-icon!='file']";
            }

            public class Button
            {
                public static string DeleteFile(string file) => $"//div[contains(@class,'MuiGrid-root MuiGrid-container css') and .='{file}']//*[(local-name()='svg') and @data-icon='trash']//*[local-name()='path']";
                public const string BrowseFiles = "id('file-upload-input')";
                public const string Submit = "//button[text()='Submit']";
            }
        }
       
        public UploadAndSubmit(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.ClaimNumber);
                GetElement(XPath.Button.BrowseFiles);
                GetElement(XPath.Button.Submit);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Upload Invoice - Upload and submit");
            return true;
        }

        private void UploadFiles(List<string> files)
        {
            string filePath = null;

            //Use the Local File Detector method that enables file transfers from your machine to the BrowserStack remote browser.
            if (Config.Get().IsCrossBrowserDeviceTestingEnabled)
            {
                if ((_browser.DeviceName == TargetDevice.MacBook) || (_browser.DeviceName == TargetDevice.Windows11))
                {
                    LocalFileDetector detector = new LocalFileDetector();
                    var allowsDetection = _browser.Driver as IAllowsFileDetection;
                    if (allowsDetection != null)
                    {
                        allowsDetection.FileDetector = detector;
                    }
                }
            }

            //For uploading multiple files at once, we need to concatinate the files using new line delimiter between each file
            foreach (var item in files)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", item);

                if (File.Exists(path))
                {
                    filePath = string.Join(" \n ", path, filePath);
                    filePath = filePath.Replace("\\", "/");
                }
                else
                {
                    Reporting.Error($"Could not find file {item} to upload, check test data.");
                }
            }

            filePath = filePath.EndsWith(" \n ") ? filePath.Remove(filePath.LastIndexOf(" \n ")) : filePath;
            GetElement(XPath.Button.BrowseFiles).SendKeys(filePath);
        }

        //Upload all the files and verify all files are uploaded successfully
        public void UploadFileAndCheckStatus(ClaimUploadFile claim)
        {
            var remainingFiles = DataHelper.GetRemainingUploadDocumentCount(claim.PersonId, claim.ClaimNumber);
            VerifyRemainingFilesMessage(remainingFiles);
            UploadFiles(claim.File);
            int afterUploadRemainingFile = (remainingFiles - claim.File.Count);

            if (UploadFileSuccessfully(claim.File))
            {
                _browser.PercyScreenCheck(DocumentUpload.AfterUploadFinished, GetPercyIgnoreCSS());
                Reporting.Log("Upload File successfully", _browser.Driver.TakeSnapshot());
                VerifyRemainingFilesMessage(afterUploadRemainingFile);
            }
            else
            {
                Reporting.Error("Upload File failed", _browser.Driver.TakeSnapshot());
            }
        }

        //Upload an unsupported file and verify the error message
        public void UploadFileError(ClaimUploadFile claim)
        {
            VerifyRemainingFilesMessage(ClaimsGeneral.MaxNumberOfFile);
            UploadFiles(claim.File);
            Reporting.Log("File upload error message", _browser.Driver.TakeSnapshot());

            if (claim.File.FirstOrDefault() == FileType.WordFile)
            {
                Reporting.AreEqual(Constants.UnsupportedFileText, GetInnerText(XPath.Field.ErrorText), "Unsupported file error message: ");
                _browser.PercyScreenCheck(DocumentUpload.UnsupportedFileErrorMessage, GetPercyIgnoreCSS());
            }
            else if (claim.File.FirstOrDefault() == FileType.PDFFile10MB)
            {
                Reporting.AreEqual(Constants.ExceedsMaxFileSizeText, GetInnerText(XPath.Field.ErrorText), "Exceeds file size error message: ");
                _browser.PercyScreenCheck(DocumentUpload.ExceedsMaximumFileSizeErrorMessage, GetPercyIgnoreCSS());
            }           
        }

        /// <summary>
        /// Delete files one by one from the list
        /// </summary>
        /// <param name="alreadyUploadedFiles"> The list of  files which are already uploaded </param>
        /// <param name="filesToBeDeleted"> The list of files which will be deleted </param>
        public void DeleteFiles(List<string> alreadyUploadedFiles, List<string> filesToBeDeleted)
        {            
            Thread.Sleep(1000);
            int remainingFileNumber = (ClaimsGeneral.MaxNumberOfFile - alreadyUploadedFiles.Count);

            foreach (var file in filesToBeDeleted)
            {
                DeleteFile(file);
                remainingFileNumber = remainingFileNumber + 1;
                VerifyRemainingFilesMessage(remainingFileNumber);
            }
        }

        public void UploadInvoiceAndSubmit(ClaimUploadFile claim)
        {
            _browser.PercyScreenCheck(DocumentUpload.UploadAndSubmit, GetPercyIgnoreCSS());
            ClickSubmit();
            _browser.PercyScreenCheck(DocumentUpload.NoFileUploadedErrorMessage, GetPercyIgnoreCSS());

            UploadFileAndCheckStatus(claim);
            ClickSubmit();
        }

        public void ClickSubmit()
        {
            ClickControl(XPath.Button.Submit);
        }

        //Delete each file
        private void DeleteFile(string file)
        {
            bool success = false;
            ClickControl(XPath.Button.DeleteFile(file));

            var endTime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            do
            {
                try
                {
                    if (!IsControlDisplayed(XPath.UploadedFile.FileName(file)))
                    {
                        success = true;
                        break;
                    }
                }
                catch
                {
                    Reporting.Log($"Unable to delete file - {file}");
                }
                Thread.Sleep(1000);

            } while (DateTime.Now < endTime);

            if (success)
            {
                Reporting.Log($"{file} deleted successfully", _driver.TakeSnapshot());
            }
            else
            {
                Reporting.Log($"Failed to delete - {file}");
            }

        }

        //Check file uploaded successfully
        private bool UploadFileSuccessfully(List<string> file)
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            var success = false;
            foreach (var item in file)
            {
                do
                {
                    try
                    {
                        var control = GetElement(XPath.Icon.UploadFileStatus(item));
                        if (control.GetAttribute("data-icon") == "trash")
                        {
                            Reporting.Log($"{string.Format(item)} uploaded successfully", _driver.TakeSnapshot());
                            success = true;
                            break;
                        }
                    }
                    catch
                    {
                        Reporting.Log($"Unable to detect file upload status icon");
                    }
                    Thread.Sleep(1000);

                } while (DateTime.Now < endTime);
            }

            return success;
        }


        //Based on the how many files are uploade, verify the message        
        private void VerifyRemainingFilesMessage(int number)
        {            
           _browser.PercyScreenCheck(DocumentUpload.RemainingFileMessage(number), GetPercyIgnoreCSS());
            if (number == 0)
            {
                Reporting.AreEqual(Constants.MaxFileUploadText, GetInnerText(XPath.Field.RemainingFiles), "Remaining number of files");
            }
            else if (number == 1)
            {
                Reporting.AreEqual(Constants.RemainingFileText("one more file"), GetInnerText(XPath.Field.RemainingFiles), "Remaining number of files");
            }
            else if(number > 1 && number < 5)
            {
                Reporting.AreEqual(Constants.RemainingFileText($"{number} more files"), GetInnerText(XPath.Field.RemainingFiles), "Remaining number of files");
            }
            else if (number == 5)
            {
                Reporting.AreEqual(Constants.RemainingFileText($"{number} files"), GetInnerText(XPath.Field.RemainingFiles), "Remaining number of files");
            }
            else
            {
                Reporting.Error("Maximum 5 files are allowed");
            }
        }

        private List<string> GetPercyIgnoreCSS() =>
            new List<string>()
            {
               "#claimNumberDisplay span"
            };
    }
}
