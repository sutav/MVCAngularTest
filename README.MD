Front-End and WebApi are 2 separate applications as they need 2 different sets of development tools
To run Front End you need to navigate to MVCAngularTest.FrontEnd folder  using command line and run 3 commands

npm install -g @angular/cli
npm install
ng serve

Prior to running ng serve you will need to start MVCAngularTest.WebApi by running it VisualStudio - 
by default it will run on IIS express - http://localhost:60737/
If you like to use IIS then front-end variable needs to be updated in api-urls.ts to point to your WebApi urls


Dependency injection implemented using Unity
Data Access implemented using EntityFramework with repository pattern
Logging via Serilog
Front-End implemented using Angular 6 based on Angular CLI
For the purpose of this test exercise there is no security on WebApi calls
CORS was enabled on WebApi to allow cross-domain calls


By default connection to database is disabled and data is displayed from hard-coded list of strings. 
To enable set in web.config ConnectToDb = true
Run the script from solution folder " scripts" db.sql to create a table and initial data
Update ConnectionString to point to your database
 

