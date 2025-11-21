## QA Test Automation project

### What is here?
#### "Supporting" sub-folders

##### BrowserUpdates

Scripting to prompt the Web Browser on Agent Machines to update, more information [here](https://github.com/racwa/raci-test-automation-selenium/blob/main/Supporting/ExtentReports/BAT_for_extracting_reports/README.MD.
##### ExtentReports
Scripting to automate extraction of ExtentReports from the sub-folders produced by Azure after a run. See [this README.MD](https://github.com/racwa/raci-test-automation-selenium/blob/main/Supporting/ExtentReports/BAT_for_extracting_reports/README.MD) for more information.
##### SQL
This folder hosts copies of SQL queries used to support test automation, or for test automation activities that have fallen out of scope and now just preserved for posterity.

#### "Automation" folder
These are the Selenium .Net automation tests.

### QuickStart:
- You will need a Visual Studio (VS) install. You can use VS2017 or newer, and it should be a Professional/Enterprise version, as VS Code doesn't support solution files. (Within RAC you should not use VS Community as we don't qualify under the T&Cs).
- Use VS to open the solution file (./Automation/TestCollection_B2C.sln)
- You will need to build the solution (press F6, or choose 'Build Solution' from the Build dropdown menu)
- To view the test cases, you'll need to open the Test Explorer, by going to Test -> Windows -> Test Explorer.
- Within the Test Explorer sidebar, you can select single or groups of tests and run them there.
- The targeted environment for a test is set by the 'config.json' that you will find in the test project folder.

### Some maintenance items:
If you find motor claims tests failing in regards to assigned service providers and expected skills, then this means that our Shield environment has not had new quotas set for service providers for the current environment. You will know this to be the case if these failures start on the 1st of a new month.

The service providers we use are:
#### Allocate Glazier
- Contact ID: 2908773 - Novus Autoglass          (WA region)

#### Claim Status Motor tests
- Contact ID: 1599689 - Nulife Panel & Paint     (Geraldton/Meekatharra region)
- Contact ID: 4219841 - Fenner Collision         (Geraldton/Meekatharra region)

#### Anon Motor Claim allocate repairer
- Contact ID: 3063124 - Advantage Panel & Paint  (Nedlands region)
- Contact ID: 2882696 - Osborne Smash Repairs    (Osborne Park region)