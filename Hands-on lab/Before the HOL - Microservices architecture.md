![](https://github.com/Microsoft/MCW-Template-Cloud-Workshop/raw/master/Media/ms-cloud-workshop.png "Microsoft Cloud Workshops")

<div class="MCWHeader1">
Microservices architecture
</div>

<div class="MCWHeader2">
Before the hands-on lab setup guide
</div>

<div class="MCWHeader3">
May 2019
</div>


Information in this document, including URL and other Internet Web site references, is subject to change without notice. Unless otherwise noted, the example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious, and no association with any real company, organization, product, domain name, e-mail address, logo, person, place or event is intended or should be inferred. Complying with all applicable copyright laws is the responsibility of the user. Without limiting the rights under copyright, no part of this document may be reproduced, stored in or introduced into a retrieval system, or transmitted in any form or by any means (electronic, mechanical, photocopying, recording, or otherwise), or for any purpose, without the express written permission of Microsoft Corporation.

Microsoft may have patents, patent applications, trademarks, copyrights, or other intellectual property rights covering subject matter in this document. Except as expressly provided in any written license agreement from Microsoft, the furnishing of this document does not give you any license to these patents, trademarks, copyrights, or other intellectual property.

The names of manufacturers, products, or URLs are provided for informational purposes only and Microsoft makes no representations and warranties, either expressed, implied, or statutory, regarding these manufacturers or the use of the products with any Microsoft technologies. The inclusion of a manufacturer or product does not imply endorsement of Microsoft of the manufacturer or product. Links may be provided to third party sites. Such sites are not under the control of Microsoft and Microsoft is not responsible for the contents of any linked site or any link contained in a linked site, or any changes or updates to such sites. Microsoft is not responsible for webcasting or any other form of transmission received from any linked site. Microsoft is providing these links to you only as a convenience, and the inclusion of any link does not imply endorsement of Microsoft of the site or the products contained therein.

© 2019 Microsoft Corporation. All rights reserved.

Microsoft and the trademarks listed at <https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/Usage/General.aspx> are trademarks of the Microsoft group of companies. All other trademarks are property of their respective owners.

**Contents**

<!-- TOC -->

- [Microservices architecture before the hands-on lab setup guide](#microservices-architecture-before-the-hands-on-lab-setup-guide)
    - [Requirements](#requirements)
    - [Before the hands-on lab](#before-the-hands-on-lab)
        - [Task 1: Provision Service Fabric Cluster](#task-1-provision-service-fabric-cluster)
        - [Task 2: Provision a lab virtual machine (VM)](#task-2-provision-a-lab-virtual-machine-vm)
        - [Task 3: Connect to your lab VM](#task-3-connect-to-your-lab-vm)
        - [Task 4: Install Chrome on LabVM](#task-4-install-chrome-on-labvm)
        - [Task 5: Install Service Fabric SDK for Visual Studio](#task-5-install-service-fabric-sdk-for-visual-studio)
        - [Task 6: Setup Service Fabric certificate](#task-6-setup-service-fabric-certificate)
        - [Task 7: Validate Service Fabric ports](#task-7-validate-service-fabric-ports)

<!-- /TOC -->

# Microservices architecture before the hands-on lab setup guide 

## Requirements

1.  Microsoft Azure subscription must be pay-as-you-go or MSDN

    -   Trial subscriptions will not work.

2.  A virtual machine configured with:

    -   Visual Studio 2017 Community edition, or later

    -   Azure Development workload enabled in Visual Studio 2017 (enabled by default on the VM)

    -   Service Fabric SDK 3.3 or later for Visual Studio 2017

    -   Google Chrome browser (Swagger commands do not work in IE)

    -   PowerShell 3.0 or higher (v5.1 already installed on VM)

## Before the hands-on lab

Duration: 45 minutes

Synopsis: In this exercise, you will set up your environment for use in the rest of the hands-on lab. You should follow all the steps provided in the Before the hands-on lab section to prepare your environment before attending the hands-on lab.

>**IMPORTANT**: Most Azure resources require unique names. Throughout these steps, you will see the word "SUFFIX" as part of resource names. You should replace this with your Microsoft alias, initials, or other value to ensure the resource is uniquely named.

### Task 1: Provision Service Fabric Cluster

In this task, you will provision the Service Fabric Cluster in Azure.

1.  In the Azure portal, select +Create a Resource, then type "Service Fabric" into the Search the Marketplace box. Select Service Fabric Cluster from the results.

    ![In the Azure Portal, in the left pane, Create a resource is selected. In the Marketplace pane, Everything is selected. In the Everything pane, Service Fabric is typed in the search field, and under Results, Service Fabric Cluster is circled.](media/b4-image5.png "Azure Portal")

2.  On the Service Fabric Cluster blade, select Create.

3.  On the Basics blade of the Create Service Fabric cluster screen, enter the following:

-   Cluster name: Enter **contosoeventssf-SUFFIX**, replacing SUFFIX with your alias, initials, or another value to make the name unique (indicated by a green check in the text box).

-   Operating system: Set to WindowsServer 2016-Datacenter.

-   Username: Enter **holuser**.

-   Password: Enter **Password.1!!**

-   Subscription: Select the subscription you are using for this lab.

-   Resource Group: Select Create new, and enter **hands-on-lab** for the resource group name. You can add -SUFFIX, if needed to make resource group name unique. This is the resource group you will use for all resources you create for this hands-on lab.

-   Location: Select the region to use. Select the closest region to your current location.

-   Select OK.

    ![On the Basics blade, all fields are set to the previously defined settings.](media/b4-image6.png "Basics blade")

4.  On the Cluster configuration blade, set the following:

-   Node type count: **Select 1**.

-   Node type 1 (Primary): **Select to configure required settings**. On the Node type configuration blade enter:

    -   Node type name: Enter **Web**.

    -   Durability tier: Leave **Bronze** selected.

    -   Virtual machine size: Select a VM size of **D1\_V2 Standard** and select Select on the Choose a size blade.

        ![On the Cluster configuration blade, under Virtual Machine Size, D1\_V2 Standard is indicated.](media/ClusterConfigurationBlade.png "Cluster configuration blade")
       

    -   Single node cluster: Leave unchecked.

    -   Initial VM scale set capacity: Leave set to **5**.

    -   Custom endpoints: Enter **8082**. This will allow the Web API to be accessible through the cluster.

    -   Enable reverse proxy: Leave unchecked.

    -   Configure advanced settings: Leave unchecked.

    -   Select OK on the Node type configuration blade.

    -   Select OK on the Cluster configuration blade.

    ![The Create Service Fabric cluster blade, Cluster configuration blade, and Node type configuration blade all display with the previously defined settings.](media/b4-image8.png "Three blades")

5.  On the Security blade, you can provide security settings for your cluster. This configuration is completed up front, cannot be changed later. Set the following:

-   Configuration Type: Leave "Basic" selected.

-   Key vault: Select to configure required settings. On the Key vault configuration blade select "Create a new vault".

-   On the "Create key vault" configuration blade enter:

    -   Name: **hands-on-lab-SUFFIX**

    -   Resource Group: Select "Create new" and set the name as **hands-on-lab**
    
    -   Location: Use the same location as the first resource group you created.

        ![Create a new vault in selected in the Key vault blade, and the values specified above are entered into the Create key vault blade.](media/b4-image9.png "Key vault and Create key vault blades")

-   Select "Create" on the Create key vault configuration blade. Wait for the key vault deployment to complete.

-   When the key vault deployment completes you will return to the Security configuration blade. You will see a warning that the key vault is not enabled for deployment. Follow these steps to resolve the warning:

    -   Choose "Edit access policies for hands-on-lab-SUFFIX".

    -   In the Access policies configuration blade, choose the link "Click to show advanced access policies".

    -   Check the "Enable access to Azure Virtual Machines for deployment" checkbox.

    -   Choose "Save". When the key vault update completes, close the Access policies blade.

        ![Basic Configuration Type is selected on the Security blade, and Edit access policies for hands-on-lab-SUFFIX is selected. Enable access to Azure Virutal Machines for deployment is checked in the Access policies blade on the right.](media/b4-image10.png "Security and Access policies blades")

    -   Enter "hands-on-lab-SUFFIX" as the certificate name. Then choose OK on the Security configuration blade.

6.  On the Summary blade, review the summary, and select Create to begin provisioning the new cluster.

    ![The Summary blade displays with the message that Validation has passed.](media/b4-image11.png "Summary blade")

7.  It can take up to 30 minutes or more to provision your Service Fabric Cluster. You can move on to the next task while you wait.

>**Note**: If you experience errors related to lack of available cores, you may have to delete some other compute resources, or request additional cores to be added to your subscription, and then try this again.

### Task 2: Provision a lab virtual machine (VM)

In this task, you will provision a virtual machine (VM) in Azure. The VM image used will have Visual Studio Community 2017 installed.

1.  Launch a web browser and navigate to the [Azure portal](https://portal.azure.com/).

2.  Select +Create a Resource, then type "Visual Studio" into the search bar. Select Visual Studio Community 2017 (latest release) on Windows Server 2016 (x64) from the results.

    ![In the left pane of the Azure portal, Create a resource is circled. In the Marketplace blade, Everything is selected. In the Everything blade, the search field contains Visual Studio. Under Results, Visual Studio Community 2017 on Windows Server 2016 (x64) is circled.](media/b4-image12.png "Azure portal")

3.  On the blade that comes up, ensure the deployment model is set to Resource Manager and select Create.

    ![Resource Manager displays in the the Select a deployment model field.](media/b4-image13.png "Select a deployment model field")

4.  Set the following configuration on the Basics tab:

    -   Name: Enter LabVM.

    -   VM disk type: Leave Premium SSD selected.

    -   Username: Enter **holuser**.

    -   Password: Enter **Password.1!!**

    -   Subscription: Select the subscription you are using for this lab.

    -   Resource group: Select Use existing, and select the hands-on-labs resource group created previously.

    -   Location: Select the region you are using for resources in this lab.

        
5.  Select Change Size

6.  On the Select a VM Size blade, enter d2s into the search text field. This machine won't be doing much heavy lifting, so selecting D2S\_V3 Standard is a good baseline option.

    ![On the Choose a size blade, d2s is entered into the search text field.](media/change-lab-vm-size.png "Choose a size blade")
    
7.  Within the **INBOUND PORT RULES** section, choose RDP (3389) from the Select public inbound ports dropdown.

8.  Accept all the remaining default values on the Basic blade and select Review + Create.

    ![The Basics blade displays with the fields set to the previously stated settings.](media/lab-vm-basics-blade.png "Basics blade ")

9.  Select Create on the Create blade to provision the virtual machine.
    
    ![The Validation screen displays with the Standard D2s v3 offer details.](media/vm-validation-passed-create.png "Validation Passed")

10.  It may take 10+ minutes for the virtual machine to complete provisioning.

### Task 3: Connect to your lab VM

In this step, you will open an RDP connection to your Lab VM and disable Internet Explorer Enhanced Security Configuration.

1.  Connect to the Lab VM (If you are already connected to your Lab VM, skip to Step 9).

2.  From the side menu in the Azure portal, select Resource groups, then enter your resource group name into the filter box, and select it from the list.

    ![In the Azure Portal, Resource groups pane, hands-on is typed in the search field, and under Name, hands-on-labs is circled.](media/b4-image17.png "Azure Portal, Resource groups pane")

3.  Next, select your lab virtual machine, LabVM, from the list.

    ![In the Name list, the LabVM Virtual Machine is circled.](media/b4-image18.png "Name list")

4.  On your Lab VM blade, select Connect from the top menu.

    ![The Connect button is circled on the lab VM blade top menu.](media/b4-image19.png "Lab VM blade top menu")

5.  Download and open the RDP file.

6.  Select Connect on the Remote Desktop Connection dialog.

    ![In the Remote Desktop Connection Dialog Box, the Connect button is circled.](media/b4-image20.png "Remote Desktop Connection Dialog Box")

7.  Enter the following credentials (or the non-default credentials if you changed them):

    a.  Username: Enter **holuser**

    b.  Password: Enter **Password.1!!**

    ![The Windows Security Credentials page displays](media/b4-image21.png "Windows Security Credentials page")

8.  Select Yes to connect, if prompted that the identity of the remote computer cannot be verified.

    ![In the Remote Desktop Connection dialog box, a warning states that the identity of the remote computer cannot be verified, and asks if you want to continue anyway. At the bottom, the Yes button is circled.](media/b4-image22.png "Remote Desktop Connection dialog box")

9.  Once logged in, launch the Server Manager. This should start automatically, but you can access it via the Start menu if it does not start.

    ![The Server Manager tile is circled in the Start Menu.](media/b4-image23.png "Start Menu")

10. Select Local Server, then select On (might also display Off) next to IE Enhanced Security Configuration.

    ![In Server manager, in the left pane, Local Server is selected. In the right, Properties pane, IE Enhanced Security Configuration is circled, and a callout arrow points to On.](media/b4-image24.png "Server manager")

11. In the Internet Explorer Enhanced Security Configuration dialog, select Off under Administrators and under Users, then select OK.

    ![In the the Internet Explorer Enhanced Security Configuration dialog box, under Administrators and under Users, the Off button is selected and circled. ](media/b4-image25.png "Internet Explorer Enhanced Security Configuration dialog box")

12. Close the Server Manager.

### Task 4: Install Chrome on LabVM

In this task, you will install the Google Chrome browser on your Lab VM.

1.  On your Lab VM, open a web browser, and navigate to <https://www.google.com/chrome/browser/desktop/index.html>, and select Download Chrome.

    ![Screenshot of the Download Chrome button.](media/b4-image26.png "Download Chrome button")

2.  Select Accept and Install on the terms of service screen.

    ![The Download Chrome for Windows window displays.](media/b4-image27.png "Download Chrome for Windows ")

3.  Select Run on the Application Run -- Security Warning dialog.

    ![The Run button is circled in the Application Run -- Security Warning dialog box.](media/b4-image28.png "Application Run ??? Security Warning dialog box")

4.  Select Run again, on the Open File -- Security Warning dialog.

    ![In the Open File -- Security Warning dialog box, a message asks if you want to run the file. At the bottom, the Run button is circled.](media/b4-image29.png "Open File ??? Security Warning dialog box")

5.  Once the Chrome installation completes, a Chrome browser window should open. For ease, you can use the instructions in that window to make Chrome your default browser.

### Task 5: Install Service Fabric SDK for Visual Studio

In this task, you will install the latest Service Fabric SDK for Visual Studio 2017 on your Lab VM.

1.  On your Lab VM, open a browser, and navigate to <https://docs.microsoft.com/azure/service-fabric/service-fabric-get-started>

2.  Scroll down on the page to the Install the SDK and tools section and select Install the Microsoft Azure Service Fabric SDK under the To use Visual Studio 2017 heading.

    ![In the Install the SDK and tools section, the link to Install the Microsoft Azure Service Fabric SDK is circled.](media/b4-image30.png "Install the SDK and tools section")

3.  Run the downloaded executable and select Install in the Web Platform Installer screen.
   
    ![The Web Platform Installer window displays the information for Microsoft Azure Service Fabric SDK - 3.1.283.](media/install-service-fabric-sdk.png "Web Platform Installer window")


4.  On the Prerequisites screen, select I Accept.

    ![The Web Platform Installer Prerequisites window lists the file name and file size to be downloaded. At the bottom, the I Accept button to download additional software and review the license is circled.](media/b4-image32.png "Web Platform Installer Prerequisites window")

5.  Select Finish when the install completes.

    ![The Web Platform Installer Finish window shows that the installation was successful, and lists the products that were installed.](media/b4-image33.png "Web Platform Installer Finish window")

6.  Select Exit on the Web Platform installer to close it.

7.  Restart the VM to complete the installation and start the local Service Fabric cluster service.

### Task 6: Setup Service Fabric certificate

When you create a new Service Fabric Cluster using the portal, a secure cluster is deployed. In order to later on be able to make use of it, a certificate setup is required.

In this task, you will download the required certificate and install it on your Lab VM.

1.  In the Azure portal, navigate to the Resource Group you created previously and where you created the Key vault that supports the cluster.

2.  Select the key vault from the list of resources in the resource group.

    ![In the Azure Portal Resource Group pane, in the list of resources, the hands-on-lab-SUFFIX key vault is circled.](media/b4-image34.png "Resource group list")

3.  Under the Settings category in the menu, select Certificates and then select the existing certificate.

    ![In the Settings section, Certificates and the existing certificate are circled.](media/b4-image35.png "Certificates")

4.  Select the Current Version of the existing certificate.

    ![In the certificate list, the existing certificate is circled.](media/b4-image36.png "Existing certificate")

5.  In the certificate information blade, select Download in PFX/PEM format and save the certificate.

    ![In the certificate information blade, download the private certificate.](media/b4-image37.png "Download certificate")

6.  Copy the downloaded certificate into the Lab VM.

7.  On the Lab VM, double-click the copied certificate to initiate its installation. Select Local Machine as the Store Location and select Next.

    ![Double-click the certificate to install, select Local Machine and select Next.](media/b4-image38.png "Certificate Import Wizard")

8.  Select Next.

    ![Select Next.](media/b4-image39.png "File to import")

9.  Select Next.

    ![Select Next.](media/b4-image40.png "Private key protection")

10. Select Next.

    ![In the Certificate Store, select Next.](media/b4-image41.png "Certificate Store")

11. Select Finish.

    ![In the review panel, select next.](media/b4-image42.png "Review panel")

12. When the import finishes successfully, select OK.

    ![Import success.](media/b4-image43.png "Import successful")

13. On the Lab VM, double-click the copied certificate once again to initiate its installation. Select Current User as the Store Location and select Next.

    ![Double-click the certificate to install, select Local Machine and select Next.](media/b4-image44.png "Certificate Import Wizard")

14. Select Next.

    ![Select Next.](media/b4-image39.png "File to import")

15. Select Next.

    ![Select Next.](media/b4-image40.png "Private key protection")

16. Select Next.

    ![In the Certificate Store, select Next.](media/b4-image41.png "Certificate Store")

17. Select Finish.

    ![In the review panel, select next.](media/b4-image42.png "Review panel")

18. When the import finishes successfully, select OK.

    ![Import success.](media/b4-image43.png "Import successful")

### Task 7: Validate Service Fabric ports

Occasionally, when you create a new Service Fabric Cluster using the portal, the ports that you requested are not created. This will become evident when you try to deploy and run the Web App, because the required ports will not be accessible through the cluster.

In this task, you will validate that the ports are open and if not, fix the issue.

1.  In the Azure portal, navigate to the Resource Group you created previously, and where you created the cluster. If your Service Fabric cluster is still deploying, do not proceed to the next step until the deployment is completed.

2.  Select the load balancer from the list of resources in the resource group.

    ![In the Azure Portal Resource Group pane, in the list of resources, the LB-contosoeventssf-SUFFIX-Web load balancer is circled.](media/b4-image45.png "Resource group list")

3.  Under the Settings category in the menu, select Health probes.

    ![In the Settings section, Health probes is circled.](media/b4-image46.png "Settings section")

4.  Verify if a probe exists for port 8082, and that it is "Used By" a load balancing rule. If both of these are true, you can skip the remainder of this task. Otherwise, proceed to the next step to create the probe and load-balancing rule.

    ![In the list of health probes, three health probes display. For AppPortProbe1, its Port (8082), and Used by value (AppPortLBRule1) are circled.](media/b4-image47.png "Health probes list")

5.  Select +Add on the Health probes blade.

    ![Screenshot of an Add button.](media/b4-image48.png "Add button")

6.  On the Add health probe blade, enter the following:

    -   Name: Enter **WebApiPortProbe**.

    -   Protocol: Select TCP.

    -   Port: Enter **8082**.

    -   Interval: Leave the default value.

    -   Unhealthy threshold: Leave the default value.

    -   Select OK to create the probe.

        ![Fields on the Add health probe blade are set to the previously listed settings.](media/b4-image49.png "Add health probe blade")

7.  Once the Health probe is added (this can take a few minutes to update), you will create a rule associated with this probe. Under the Settings block in the menu, select Load balancing rules.

    ![In the Settings section, Load balancing rules are circled.](media/b4-image50.png "Settings section")

8.  Select +Add on the Load balancing rules blade.

    ![Screenshot of an Add button.](media/b4-image48.png "Add button")

9.  On the Add Load balancing rules blade, enter the following:

    -   Name: Enter **LBWebApiPortRule**.

    -   IP Version: Leave IPv4 selected.

    -   Frontend IP address: Leave the default value selected.

    -   Protocol: Leave as TCP.

    -   Port: Set to **8082**.

    -   Backend port: Set to **8082**.

    -   Backend pool: Leave the default value selected.

    -   Health probe: Select the WebApiPortProbe you created previously.

    -   Leave the default values for the remaining fields, and Select OK.

    ![Fields on the Add load balancing rule blade are set to the previously defined settings.](media/b4-image51.png "Add load balancing rule blade")

10. If you get an error notification such as "Failure to create probe", ignore this, but just go check that the probe indeed exists. It should. You now have a cluster ready to deploy to and expose 8082 as the Web API endpoint / port.

You should follow all steps provided *before* performing the Hands-on lab.
