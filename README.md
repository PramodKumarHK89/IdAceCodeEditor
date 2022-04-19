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

  ![image](https://user-images.githubusercontent.com/62542910/163817828-2bc0c55e-e468-4bd2-8095-6309ca06c697.png)

- After expanding the desired option(From the context of this article, it is sign-in flow in .NET core), you will see a `configure` button. Click on it to setup the sample code on your local machine. Please refer below screenshot

  ![image](https://user-images.githubusercontent.com/62542910/163818358-fe3bdab7-3a94-4472-b920-01d0d7bf7f13.png)

- As a result of clicking the `configure` button, app will popup a Azure AD authetication prompt as shown in the below screenshot. Please enter the credential of the tenant under which you want to create the app registration. 

  ![image](https://user-images.githubusercontent.com/62542910/163818736-727e0623-1369-4446-93d2-c06369f7718a.png)

- There could be two sceanrios here depending on the user type in the tenant. 
  - If you are a normal user in the tenant, you may get the below consent prompt depending on the user consent settings in the azure portal. Please refer the link https://docs.microsoft.com/en-us/azure/active-directory/manage-apps/configure-user-consent?tabs=azure-portal. You may have to wait until, your admin approves the request. 
     
     ![image](https://user-images.githubusercontent.com/62542910/163821155-6860cc41-3bd3-4e6d-af42-f635996d7658.png)
     
  - If you have entered the admin credential, then you will see the below prompt
  
    ![image](https://user-images.githubusercontent.com/62542910/163821326-45454624-6aa4-4068-993e-4b87a0dbf4d7.png)

- Once you pass the above hurdle which is making sure that, app has the enough permission to create the app registration, tool will do the app creation and also downloads the sample code for you. Please refer the below screenshot. 

  ![image](https://user-images.githubusercontent.com/62542910/163822328-75f29c70-de47-4ad0-92ca-cc64303a29bc.png)
  
  - The section heiglighted in (1) will list the  prerequites to run the sample code on your local machine. 
  - The section heiglighted in (2) will provide the detailed instructions on how the sample would work.
  - The section heiglighted in (3) will open the local folder where the code is downloaded.

- You are good to go now to test your sample/POC. 

