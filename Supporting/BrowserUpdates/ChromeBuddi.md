# ChromeBuddi

## What's it for?

As discussed recently in our Test Engineer chat, lack of direct access to the test automation execution agent VMs means when we have to update Chromedriver we often have to ask someone else to remote into the VM and prompt the update to Google Chrome.

The intent of this script is to open Chrome and view the chrome://settings/help page. This should trigger a check for available updates and update if available.

## Where's the code?

Introduced with [PR 100 on automation-testing-experiments](https://bitbucket.org/racwa/automation-testing-experiments/pull-requests/100) as it shares code with the KeyBuddi family of applications. The main script being ChromeBuddi.ahk and the other files involved in that PR are either assisting with or resulting from the compilation of the .exe

