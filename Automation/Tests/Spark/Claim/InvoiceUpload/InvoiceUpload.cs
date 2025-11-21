using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.ActionsAndValidations.Claims;
using UIDriver.Pages;
using System.Collections;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.TestData.Claim;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;

namespace Spark.Claim.Motor
{ 
    [Property("Functional", "Invoice Upload")]
    [Parallelizable(ParallelScope.Fixtures)]
    public  class InvoiceUpload: BaseUITest
    {
        private List<string> _motorGlassClaims;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Invoice Upload");
            _motorGlassClaims = ShieldMotorClaimDB.GetOpenMotorGlassClaims();
        }

        #region Test Cases

        
        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), 
            Category(TestCategory.VisualTest), Category(TestCategory.Regression)]
        [Test(Description = "Verify maximum file limit reached error page")]
        public void INSU_T214_CMG_LoginWithClaimNumber_PolicyCoowner_InvoiceUpload_FileLimitReached()
        {
            List<string> file = new List<string>()
            {
                FileType.PNGFile,
                FileType.PDFFile,
                FileType.JPGFile1,
                FileType.JPGFile2,
                FileType.JPGFile3
            };
            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim);
            ActionClaimUploadInvoice.UploadAndSubmitInvoice(_browser, claim);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim);

            VerifyUploadInvoiceInShield(claim);
            _browser.CloseBrowser();            
           
            LaunchPage.OpenInvoiceUploadErrorPage(_browser, claim);
            ActionClaimUploadInvoice.VerifyMaximumFileLimitReachedErrorPage(_browser, claim.ClaimNumber);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), 
            Category(TestCategory.VisualTest), Category(TestCategory.Regression)]
        [Test(Description = "Open Invoice Upload app and upload 5 files then delete 2 files and submit the claim")]
        public void INSU_T215_CMG_LoginWithClaimNumber_PolicyCoowner_InvoiceUpload_UploadAndDelete()
        {
            List<string> file = new List<string>()
            {
                FileType.PNGFile,
                FileType.PDFFile,
                FileType.JPGFile1,
                FileType.JPGFile2,
                FileType.JPGFile3
            };
            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim);
            ActionClaimUploadInvoice.UploadFiles(_browser, claim);

            List<string> deleteFile = new List<string>
            {
                FileType.JPGFile2,
                FileType.JPGFile3
            };
            claim.File = deleteFile;
            ActionClaimUploadInvoice.DeleteFiles(_browser, alreadyUploadedFiles: file, filesToBeDeleted: deleteFile);
            ActionClaimUploadInvoice.SubmitFiles(_browser);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim);

            var uploadedFiles = file.Except(deleteFile).ToList();
            claim.File = uploadedFiles;

            Reporting.LogMinorSectionHeading("Verify that full set of files are attached to claim in Shield");
            VerifyUploadInvoiceInShield(claim);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
          Category(TestCategory.VisualTest), Category(TestCategory.Regression)]
        [Test(Description = "Open Invoice Upload app and submit 3 files, then open the app again submit 2 more files for the same claim")]
        public void INSU_T212_CMG_LoginWithClaimNumber_PolicyHolder_InvoiceUpload_AddingInvoices()
        {
            List<string> file1 = new List<string>
            {
                FileType.PNGFile,
                FileType.PDFFile,
                FileType.JPGFile1
            };
            var claim = BuildTestDataMotorGlassClaimUploadFile(file1, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            Reporting.LogMinorSectionHeading("Uploading first batch of files and submit");
            LaunchPage.OpenInvoiceUploadURL(_browser, claim);           
            ActionClaimUploadInvoice.UploadAndSubmitInvoice(_browser, claim);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim);
            _browser.CloseBrowser();

            List<string> file2 = new List<string>
            {
                FileType.JPGFile2,
                FileType.JPGFile3
            };
            claim.File = file2;

            Reporting.LogMinorSectionHeading("Re-open upload URL and upload second batch of files and submit");
            LaunchPage.OpenInvoiceUploadURL(_browser, claim);           
            ActionClaimUploadInvoice.UploadAndSubmitInvoice(_browser, claim);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim);

            var allFile = file1.Concat(file2).ToList();
            claim.File = allFile;

            Reporting.LogMinorSectionHeading("Verify that full set of files are attached to claim in Shield");
            VerifyUploadInvoiceInShield(claim);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
          Category(TestCategory.Regression)]
        [Test(Description = "Lodge a new motor glass claim and upload the invoices in the same flow")]
        public void INSU_T211_CMG_LoginWithPolicyNumber_PolicyHolder_AlreadyFixed_InvoiceUpload()
        {
            var claim = BuildTestDataMotorGlassClaimAlreadyFixed(LoginWith.PolicyNumber, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageAlreadyFixed, false, false);
                        
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);

            Reporting.Log($"Verify Invoice Upload for Claim: {claim.ClaimNumber}");

            claim.ClaimUploadFile.ClaimNumber = claim.ClaimNumber;
            ActionSparkMotorGlassClaim.ClickSubmitInvoice(_browser);
            ActionClaimUploadInvoice.UploadAndSubmitInvoice(_browser, claim.ClaimUploadFile);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim.ClaimUploadFile);

            InvoiceUpload.VerifyUploadInvoiceInShield(claim.ClaimUploadFile);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
          Category(TestCategory.VisualTest)]
        [Test(Description = "Open Invoice Upload app and upload a word file, then verify the unsupported file error message")]
        public void CMG_UploadInvoice_PolicyHolder_Upload_UnsupportedFile()
        {
            List<string> file = new List<string>()
            {
                FileType.WordFile
            };

            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyHolder);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim);
            ActionClaimUploadInvoice.UploadAnVerifyError(_browser, claim);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
         Category(TestCategory.VisualTest)]
        [Test(Description = "Open Invoice Upload app and upload a 10mb pdf file, then verify the maximum file size error message")]
        public void CMG_UploadInvoice_CoPolicyOwner_Upload_LargePDF()
        {
            List<string> file = new List<string>()
            {
                FileType.PDFFile10MB
            };

            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim);
            ActionClaimUploadInvoice.UploadAnVerifyError(_browser, claim);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), 
            Category(TestCategory.VisualTest), Category(TestCategory.Regression)]
        [Test(Description = "Open the Invoice Upload app for a claim which is closed 6 months before, then verify link expired error message")]
        public void INSU_T213_CMG_LoginWithClaimNumber_PolicyCoowner_InvoiceUpload_LinkExpired()
        {
            List<string> file = new List<string>()
            {
                FileType.PDFFile
            };

            var claim = BuildTestDataMotorGlassClaimClosedUploadFile(file, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadErrorPage(_browser, claim);
            ActionClaimUploadInvoice.VerifyLinkExpiredErrorPage(_browser, claim.ClaimNumber);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
         Category(TestCategory.VisualTest)]
        [Test(Description = "Verify something went wrong error page")]
        public void CMG_Error_UploadInvoice_PolicyHolder_SomethingWentWrong()
        {
            List<string> file = new List<string>()
            {
                FileType.PDFFile
            };

            var claim = BuildTestDataInvoiceUploadInvalidPersonId(file, ClaimContactRole.PolicyHolder);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadErrorPage(_browser, claim);
            ActionClaimUploadInvoice.VerifySomethingWentWrongErrorPage(_browser, claim.ClaimNumber);
        }

        [Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass),
            Category(TestCategory.VisualTest)]
        [Test(Description = "Verify session timeout error page")]
        public void CMG_Error_UploadInvoice_CoPolicyOwner_SessionTimeOut()
        {
            List<string> file = new List<string>()
            {
                FileType.PNGFile
            };
            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyCoOwner);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim);
            LaunchPage.NavigateToInvoiceUploadSessionTimeOutPage(_browser);
            ActionClaimUploadInvoice.VerifySessionTimeOutErrorPage(_browser);

        }


        [Test, TestCaseSource(typeof(CrossBrowserAndDeviceList)), Category(TestCategory.ClaimServicing), Category(TestCategory.Motor), Category(TestCategory.Spark), 
            Category(TestCategory.Glass), Category(TestCategory.CrossBrowserAndDeviceTest)]
        public void CrossBrowserAndDevice_UploadInvoice_PolicyOwner_PNGFile(TargetDevice deviceName, TargetBrowser browserName)
        {
            List<string> file = new List<string>()
            {
                FileType.PNGFile,
                FileType.PDFFile,
                FileType.JPGFile1,
                FileType.JPGFile2,
                FileType.JPGFile3
            };

            var claim = BuildTestDataMotorGlassClaimUploadFile(file, ClaimContactRole.PolicyHolder);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();

            LaunchPage.OpenInvoiceUploadURL(_browser, claim, browserName: browserName, device: deviceName);
            ActionClaimUploadInvoice.UploadFiles(_browser, claim);
            ActionClaimUploadInvoice.DeleteFiles(_browser, alreadyUploadedFiles: file, filesToBeDeleted: file);
            ActionClaimUploadInvoice.UploadAndSubmitInvoice(_browser, claim);
            ActionClaimUploadInvoice.VerifyConfirmationPage(_browser, claim);            
        }

        #endregion


        #region Test cases helper methods

        private ClaimUploadFile BuildTestDataMotorGlassClaimUploadFile(List<string> file, string contactRole)
        {
            ClaimUploadFile testData = null;
            foreach (var claim in _motorGlassClaims)
            {
                var claimDetails = DataHelper.GetClaimDetails(claim);

                var claimContact = claimDetails.ClaimContacts.Find(x => x.ClaimContactRole == contactRole);
                if (claimContact == null)
                { continue; }

                var contact = DataHelper.GetContactDetailsViaExternalContactNumber(claimContact.ContactExternalNumber);
                var getPersonResponse = DataHelper.GetPersonFromMemberCentralByContactId(contact.Id);

                if (getPersonResponse == null || DataHelper.GetRemainingUploadDocumentCount(getPersonResponse.PersonId, claim) != 5)
                { continue; }

                testData = new ClaimUploadFile(claim, getPersonResponse.PersonId, getPersonResponse.FirstName, file);

                //Remove the used claim number from the list
                _motorGlassClaims.Remove(testData.ClaimNumber);
                break;
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        private ClaimUploadFile BuildTestDataMotorGlassClaimClosedUploadFile(List<string> file, string contactRole)
        {
            ClaimUploadFile claimInvoiceUpload = null;
            var closedMotorGlassClaims = ShieldMotorClaimDB.GetClosedMotorGlassClaimsMoreThan6MonthsOld();

            foreach (var claim in closedMotorGlassClaims)
            {
                var claimDetails = DataHelper.GetClaimDetails(claim);

                var claimContact = claimDetails.ClaimContacts.Find(x => x.ClaimContactRole == contactRole);
                if (claimContact == null)
                { continue; }

                var contact = DataHelper.GetContactDetailsViaExternalContactNumber(claimContact.ContactExternalNumber);
                var getPersonResponse = DataHelper.GetPersonFromMemberCentralByContactId(contact.Id);

                if (getPersonResponse == null)
                { continue; }

                claimInvoiceUpload = new ClaimUploadFile(claim, getPersonResponse.PersonId, contact.FirstName, file);
                break;
            }

            Reporting.IsNotNull(claimInvoiceUpload, "suitable test data has been found");
            return claimInvoiceUpload;
        }

        private ClaimUploadFile BuildTestDataInvoiceUploadInvalidPersonId(List<string> file, string contactRole)
        {
            ClaimUploadFile claimInvoiceUpload = null;
            var closedMotorGlassClaims = ShieldMotorClaimDB.GetClosedMotorGlassClaimsMoreThan6MonthsOld();

            foreach (var claim in closedMotorGlassClaims)
            {
                var claimDetails = DataHelper.GetClaimDetails(claim);

                var claimContact = claimDetails.ClaimContacts.Find(x => x.ClaimContactRole == contactRole);
                if (claimContact == null)
                { continue; }

                var contact = DataHelper.GetContactDetailsViaExternalContactNumber(claimContact.ContactExternalNumber);
                var getPersonResponse = DataHelper.GetPersonFromMemberCentralByContactId(contact.Id);

                if (getPersonResponse == null)
                { continue; }

                //remove last 2 char from personId to make it invalid
                var personId = getPersonResponse.PersonId.Substring(0, getPersonResponse.PersonId.Length - 2);
                claimInvoiceUpload = new ClaimUploadFile(claim, personId, contact.FirstName, file);

                break;
            }

            Reporting.IsNotNull(claimInvoiceUpload, "suitable test data has been found");
            return claimInvoiceUpload;
        }

        private ClaimCar BuildTestDataMotorGlassClaimAlreadyFixed(LoginWith loginType, ContactRole contactRole, MotorClaimScenario glassClaimScenario, bool onlyWindscreenDamaged, bool otherWindowGlass,
                   bool changeMobileNumber = false, bool changeEmail = false)
        {
            ClaimCar testData       = null;
            var azureTableName      = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;
            var motorPolicyEntities = DataHelper.AzureTableGetAllRecords(azureTableName)
                                                   .FindAll(x => (x.CoverType == DataHelper.GetDescription(MotorCovers.MFCO)) &&
                                                                                 (!x.IsEV)).PickRandom(5);
            var motorPoliciesForClaims = DataHelper.GetAllMotorPolicyDetails(motorPolicyEntities, azureTableName);

            List<string> file = new List<string>()
            {
                FileType.PNGFile,
                FileType.PDFFile,
                FileType.JPGFile1,
                FileType.JPGFile2,
                FileType.JPGFile3
            };

            var policiesForGlassClaims = motorPoliciesForClaims.FindAll(x => x.PolicyHolders.Any(p => p.ContactRoles.Any(c => c == contactRole)));

            foreach (var policy in policiesForGlassClaims)
            {
                var claimant = policy.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));

                if (ShieldClaimDB.GetOpenClaimCountForPolicy(policy.PolicyNumber) != 0)
                { continue; }

                var linkedMotorPolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Motor");

                testData = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(policy, MotorClaimDamageType.WindscreenGlassDamage)
                                               .LoginWith(loginType)
                                               .WithGlassDamageDetails(onlyWindscreenDamaged, otherWindowGlass, GlassDamageType.Chip, false)
                                               .WithClaimant(claimant, changeMobileNumber, changeEmail)
                                               .WithEventDateAndTime(DateTime.Now.Date.AddDays(-6).AddHours(15).AddMinutes(15))
                                               .WithClaimScenario(glassClaimScenario)
                                               .WithLinkedMotorPoliciesForClaimant(linkedMotorPolicies)
                                               .WithClaimUploadFiles(null, claimant.PersonId, claimant.FirstName, file)
                                               .Build();
                break;
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(testData.Policy.PolicyNumber, azureTableName);

            return testData;
        }

        //Currently document upload is working for Safari and Edge browser
        //implementation is pending for mobile devices
        private class CrossBrowserAndDeviceList : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { TargetDevice.MacBook, TargetBrowser.Safari };
                yield return new object[] { TargetDevice.Windows11, TargetBrowser.Edge };
            }
        }
    #endregion

        #region Verify methods
        public static void VerifyUploadInvoiceInShield(ClaimUploadFile claim)
        {
            VerifyRemainingUploadDocumentCount(claim);
            VerifyUploadedFilesInShield(claim);
            VerifyInvoiceUploadEventInShield(claim.ClaimNumber);
        }

        private static void VerifyRemainingUploadDocumentCount(ClaimUploadFile claim)
        {
            var remainingFiles = DataHelper.GetRemainingUploadDocumentCount(claim.PersonId, claim.ClaimNumber);
            Reporting.AreEqual((MaxNumberOfFile - claim.File.Count()), remainingFiles, "Remaining file upload count");

        }

        private static void VerifyUploadedFilesInShield(ClaimUploadFile claim)
        {
            var fileNamesInShield = ShieldClaimDB.GetUploadedInvoiceFileNames(claim.ClaimNumber);
            Reporting.AreEqual(claim.File.Count, fileNamesInShield.Count, "Shield has the correct number of uploaded files attached to the claim");
        }

        private static void VerifyInvoiceUploadEventInShield(string claimNumber)
        {
            var eventNames = ShieldClaimDB.GetClaimEvents(claimNumber);
            Reporting.IsTrue(eventNames.Contains(ShieldEvent.InvoiceDocumentsReceived.GetDescription()), $"{eventNames} created in Shield");
        }

        #endregion
    }
}