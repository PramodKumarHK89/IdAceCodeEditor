# IdAceCodeEditor - Tool which helps developers to setup the sample code interacting with Azure AD across multipe platforms

## How to use the tool

- Go to the [release tab](https://github.com/PramodKumarHK89/IdAceCodeEditor/releases) and download the latest IdAceCodeEditor_DATE_PREFIX.zip file. Please refer the below screenshot 
 
  ![image](https://user-images.githubusercontent.com/62542910/163988441-c29e7d9d-6503-4438-81d0-ef48a90cdd61.png)
  
- Navigate to the downloaded folder to locate the zip file. You may want to make sure that, file is unblocked before unzipping it to a folder.Please refer below screenshot.
  
  ![image](https://user-images.githubusercontent.com/62542910/163988784-5a11a3a1-4d4e-445d-a356-ecbda87b2d37.png)
  
- Extract the zip file to a local folder. To avoid path length limitations on Windows, we recommend extracting it into a directory near the root of your drive. Please refer below screenshot.

  ![image](https://user-images.githubusercontent.com/62542910/163816428-98c1bbf8-c1da-4288-9824-f71aa49dd87f.png)

- Navigate to the folder where the files are unzipped. Locate the `IdAceCodeEditor.exe` file and double click on it to execute it. Please refer the below screenshot.

  ![image](https://user-images.githubusercontent.com/62542910/163816900-a9353835-764b-4b7c-ab12-f9250ebd2888.png)

- The window should popup with the below screen. The window should let you to choose the platform in which you want to test/create the POC(Example- .NET CORE, Angular,JAVA, etc.). For the sake of articualting the usage of the tool, I'll take an example of .NET core and simple sign-in flow. Please refer the below screenshot for reference

  ![image](https://user-images.githubusercontent.com/62542910/171006054-3bf46b15-41f2-4cb6-ad6b-c8719a81d231.png)

- After expanding the desired option(From the context of this article, it is sign-in flow in .NET core), you will see three sections (1)Description of the sample along with the relavent tags (2) `configure` button with Azure AD (3) `Configure` button with manual option . Please refer below screenshot

  ![image](https://user-images.githubusercontent.com/62542910/171007604-d945607d-ce83-47a8-8846-85922006ee13.png)

- ##Configure-Connect with Azure Ad flow: 
 - If you click on the `configure- connect with Azure Ad` button, app will popup a Azure AD authetication prompt as shown in the below screenshot. Please enter the credential of the tenant under which you want to create the app registration. 

  ![image](https://user-images.githubusercontent.com/62542910/163818736-727e0623-1369-4446-93d2-c06369f7718a.png)

 - If you are a normal user in the tenant, you may get the below consent prompt depending on the user consent settings in the azure portal. Please refer the link https://docs.microsoft.com/en-us/azure/active-directory/manage-apps/configure-user-consent?tabs=azure-portal. You may have to wait until, your admin approves the request. 
     
     ![image](https://user-images.githubusercontent.com/62542910/163821155-6860cc41-3bd3-4e6d-af42-f635996d7658.png)
     
 - If you have entered the admin credential, then you will see the below prompt
  
    ![image](https://user-images.githubusercontent.com/62542910/163821326-45454624-6aa4-4068-993e-4b87a0dbf4d7.png)

 - Once you pass the above hurdle which is making sure that, app has the enough permission to list the app registrations, tool will display the lists of aps registered in the tenant. Please refer the below screenshot. 

  ![image](https://user-images.githubusercontent.com/62542910/171008725-2323caea-e309-4fb9-94a1-036bb68bbc5c.png)

  
 - Tool gives two options here. Either select the existing pp registration or create the new one. As name suggestes, if you click on the `Create` button then, it creates the app regsitration for you. Before that, it could ask Application.ReadWrite permission. Please refer below screenshot
 
  ![image](https://user-images.githubusercontent.com/62542910/171009268-b7cf82aa-da91-4464-b63c-998f205444a8.png)

 - Another option is to select the existing app registration and skip the above step. This doesnt need Application.ReadWrite permission. 
 - Both the above steps should display the below prerequites window if it is succesfully downloaded the sample.
  
   ![image](https://user-images.githubusercontent.com/62542910/171009633-2a654965-0378-4df5-8ddf-89d40b011bd6.png)
   
   - The section heiglighted in (1) will list the prerequites to run the sample code on your local machine.
   - The section heiglighted in (2) will provide the detailed instructions on how the sample would work.
   - The section heiglighted in (3) will open the local folder where the code is downloaded.
 
- ##Configure-Manual updation: 
 - This option doesnt need any permission from the tenant. You should see a below window where you are asked to update the required information to pre-populate the sample
  
   ![image](https://user-images.githubusercontent.com/62542910/171010217-1b738339-d2d6-43c1-9f0a-c23c1014161c.png)

 - After successfull configuration you will see a prereuisites window.
- You are good to go now to test your sample/POC. 

